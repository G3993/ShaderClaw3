(function(THREE) {
    const INPUTS = [
        { NAME: "speed", LABEL: "Rotation Speed", TYPE: "float", DEFAULT: 2.17, MIN: 0.0, MAX: 3.0 },
        { NAME: "trophyColor", LABEL: "Trophy Color", TYPE: "color", DEFAULT: [1.0, 0.84, 0.0, 1.0] },
        { NAME: "metalness", LABEL: "Metalness", TYPE: "float", DEFAULT: 0.78, MIN: 0.0, MAX: 1.0 },
        { NAME: "roughness", LABEL: "Roughness", TYPE: "float", DEFAULT: 0.32, MIN: 0.0, MAX: 1.0 },
        { NAME: "spotIntensity", LABEL: "Spotlight", TYPE: "float", DEFAULT: 1.8, MIN: 0.0, MAX: 8.0 },
        { NAME: "rimIntensity", LABEL: "Rim Light", TYPE: "float", DEFAULT: 1.5, MIN: 0.0, MAX: 5.0 },
        { NAME: "lightDrift", LABEL: "Light Drift", TYPE: "float", DEFAULT: 0.3, MIN: 0.0, MAX: 1.0 },
        { NAME: "audioReact", LABEL: "Audio React", TYPE: "float", DEFAULT: 0.5, MIN: 0.0, MAX: 1.0 },
        { NAME: "texture", LABEL: "Texture", TYPE: "image" },
        { NAME: "modelScale", LABEL: "Scale", TYPE: "float", DEFAULT: 0.8, MIN: 0.3, MAX: 3.0 },
        { NAME: "movement", LABEL: "Camera Orbit", TYPE: "float", DEFAULT: 1.37, MIN: 0.0, MAX: 2.0 },
        { NAME: "transparentBg", TYPE: "bool", DEFAULT: true },
        { NAME: "bgColor", LABEL: "Background", TYPE: "color", DEFAULT: [0.0, 0.0, 0.0, 1.0] }
    ];

    function create(renderer, canvas, media) {
        var scene = new THREE.Scene();
        var _bgColor = new THREE.Color(0x000000);
        scene.background = null;

        var camera = new THREE.PerspectiveCamera(45, canvas.width / canvas.height, 0.1, 100);
        camera.position.set(0, 0.5, 4);
        camera.lookAt(0, 0, 0);

        // === Lighting: dramatic top-down spotlight + rim ===

        // Main spotlight from above (like the reference poster)
        var spotlight = new THREE.SpotLight(0xffd966, 3.0, 20, Math.PI * 0.2, 0.5, 1.0);
        spotlight.position.set(0, 8, 1);
        spotlight.target.position.set(0, 0, 0);
        scene.add(spotlight);
        scene.add(spotlight.target);

        // Warm rim light from behind-left (gold edge highlighting)
        var rimLight = new THREE.DirectionalLight(0xffc040, 1.5);
        rimLight.position.set(-3, 3, -4);
        scene.add(rimLight);

        // Subtle warm fill from front-right
        var fillLight = new THREE.DirectionalLight(0xffa020, 0.3);
        fillLight.position.set(2, 1, 3);
        scene.add(fillLight);

        // Very dim ambient (keeps the trophy mostly in shadow with gold highlights)
        var ambient = new THREE.AmbientLight(0x1a1000, 0.2);
        scene.add(ambient);

        // === Trophy pivot ===
        var pivot = new THREE.Group();
        scene.add(pivot);

        // Gold material for the trophy
        var goldMaterial = new THREE.MeshStandardMaterial({
            color: new THREE.Color(1.0, 0.84, 0.0),
            metalness: 0.95,
            roughness: 0.15,
            envMapIntensity: 1.0
        });

        // Default placeholder (shown while GLB loads)
        var placeholderGeom = new THREE.CylinderGeometry(0.15, 0.25, 2.0, 16);
        var placeholder = new THREE.Mesh(placeholderGeom, goldMaterial);
        placeholder.position.y = 0;
        pivot.add(placeholder);

        var customModel = null;
        var customModelId = null;
        var _defaultModelLoaded = false;

        // Auto-load the Oscar trophy GLB (waits for deferred GLTFLoader)
        function _loadDefaultModel(model) {
            var box = new THREE.Box3().setFromObject(model);
            var center = box.getCenter(new THREE.Vector3());
            var extent = box.getSize(new THREE.Vector3()).length();
            var s = extent > 0 ? 2.5 / extent : 1;
            model.scale.multiplyScalar(s);
            model.position.copy(center).multiplyScalar(-s);
            model.traverse(function(child) {
                if (child.isMesh) child.material = goldMaterial;
            });
            customModel = model;
            customModelId = '__default__';
            placeholder.visible = false;
            pivot.add(model);
            _defaultModelLoaded = true;
        }
        function _tryLoadGLB() {
            if (typeof THREE.GLTFLoader === 'undefined') {
                // Loader not ready yet — retry after a delay
                setTimeout(_tryLoadGLB, 500);
                return;
            }
            var loader = new THREE.GLTFLoader();
            loader.load(
                'assets/oscar_trophy.glb',
                function(gltf) { _loadDefaultModel(gltf.scene); },
                undefined,
                function(err) { console.warn('Oscar trophy GLB not found, using placeholder:', err); }
            );
        }
        // Ensure deferred loaders start loading, then try
        if (typeof loadDeferredScripts === 'function') loadDeferredScripts();
        _tryLoadGLB();

        // Environment map for metallic reflections
        var envScene = new THREE.Scene();
        envScene.background = new THREE.Color(0x0a0500);
        // Add some warm lights to the env for reflections
        var envLight1 = new THREE.Mesh(
            new THREE.SphereGeometry(0.5, 8, 8),
            new THREE.MeshBasicMaterial({ color: 0xffd060 })
        );
        envLight1.position.set(3, 4, 2);
        envScene.add(envLight1);
        var envLight2 = new THREE.Mesh(
            new THREE.SphereGeometry(0.3, 8, 8),
            new THREE.MeshBasicMaterial({ color: 0xffb030 })
        );
        envLight2.position.set(-2, 3, -3);
        envScene.add(envLight2);

        var cubeRenderTarget = new THREE.WebGLCubeRenderTarget(128);
        var cubeCamera = new THREE.CubeCamera(0.1, 10, cubeRenderTarget);
        cubeCamera.update(renderer, envScene);
        goldMaterial.envMap = cubeRenderTarget.texture;

        return {
            scene: scene,
            camera: camera,
            update: function(time, values, mediaList) {
                var spd = (values.speed != null) ? values.speed : 0.3;
                var ar = (values.audioReact != null) ? values.audioReact : 0.5;
                var ld = (values.lightDrift != null) ? values.lightDrift : 0.3;

                // Audio
                var aLevel = (values._audioLevel || 0) * ar;
                var aBass = (values._audioBass || 0) * ar;
                var aMid = (values._audioMid || 0) * ar;
                var aHigh = (values._audioHigh || 0) * ar;

                // Transparent background
                var wantTransparent = values.transparentBg != null ? !!values.transparentBg : true;
                if (wantTransparent) {
                    scene.background = null;
                } else {
                    if (values.bgColor) {
                        var bg = values.bgColor;
                        _bgColor.setRGB(bg[0], bg[1], bg[2]);
                    }
                    scene.background = _bgColor;
                }

                // Update trophy material
                if (values.trophyColor) {
                    var tc = values.trophyColor;
                    goldMaterial.color.setRGB(tc[0], tc[1], tc[2]);
                }
                goldMaterial.metalness = (values.metalness != null) ? values.metalness : 0.95;
                goldMaterial.roughness = (values.roughness != null) ? values.roughness : 0.15;

                // Texture mapping (image, video, NDI — from mediaList)
                var texId = values.texture;
                if (texId && mediaList) {
                    var m = mediaList.find(function(e) { return String(e.id) === String(texId); });
                    if (m && m.threeTexture) {
                        m.threeTexture.wrapS = THREE.RepeatWrapping;
                        m.threeTexture.wrapT = THREE.RepeatWrapping;
                        m.threeTexture.needsUpdate = true;
                        if (goldMaterial.map !== m.threeTexture) {
                            goldMaterial.map = m.threeTexture;
                            goldMaterial.needsUpdate = true;
                        }
                    }
                } else if (!texId && goldMaterial.map) {
                    goldMaterial.map = null;
                    goldMaterial.needsUpdate = true;
                }

                // Spotlight intensity with audio pulse
                var spotBase = (values.spotIntensity != null) ? values.spotIntensity : 3.0;
                spotlight.intensity = spotBase * (1.0 + aBass * 1.0);

                // Rim light with audio
                var rimBase = (values.rimIntensity != null) ? values.rimIntensity : 1.5;
                rimLight.intensity = rimBase * (1.0 + aMid * 0.6);

                // Subtle light position drift over time
                var driftT = time * 0.3 * ld;
                spotlight.position.x = Math.sin(driftT * 0.7) * 1.5 * ld;
                spotlight.position.z = 1 + Math.cos(driftT * 0.5) * 1.0 * ld;
                rimLight.position.x = -3 + Math.sin(driftT * 0.4) * 1.0 * ld;

                // Load custom model from media (overrides default GLB)
                var modelMedia = mediaList && mediaList.find(function(e) {
                    return e.type === 'model' && e.threeModel;
                });
                if (modelMedia && modelMedia.id !== customModelId) {
                    // Remove old (including default model)
                    if (customModel) pivot.remove(customModel);
                    placeholder.visible = false;

                    customModel = modelMedia.threeModel.clone();
                    customModelId = modelMedia.id;

                    // Normalize scale and center
                    var box = new THREE.Box3().setFromObject(customModel);
                    var center = box.getCenter(new THREE.Vector3());
                    var extent = box.getSize(new THREE.Vector3()).length();
                    var s = extent > 0 ? 2.5 / extent : 1;
                    customModel.scale.multiplyScalar(s);
                    customModel.position.copy(center).multiplyScalar(-s);

                    // Apply gold material to all meshes
                    customModel.traverse(function(child) {
                        if (child.isMesh) {
                            child.material = goldMaterial;
                        }
                    });

                    pivot.add(customModel);
                } else if (!modelMedia && customModel && !_defaultModelLoaded) {
                    // Only remove if no default model loaded
                    pivot.remove(customModel);
                    customModel = null;
                    customModelId = null;
                    placeholder.visible = true;
                }

                // Rotation
                pivot.rotation.y = time * spd;
                // Subtle tilt with audio
                pivot.rotation.x = Math.sin(time * 0.15) * 0.03 + aBass * 0.05;

                // Scale
                var sc = (values.modelScale != null) ? values.modelScale : 1.0;
                sc *= 1.0 + aBass * 0.03; // subtle audio pulse
                pivot.scale.setScalar(sc);

                // Camera orbit
                var orbit = (values.movement != null) ? values.movement : 0.2;
                if (orbit > 0.001) {
                    var ot = time * 0.15 * orbit;
                    var angle = ot + 0.3 * Math.sin(ot * 0.6);
                    var dist = 4.0 + 0.3 * Math.sin(ot * 0.4) * orbit;
                    var camY = 0.5 + 0.4 * Math.sin(ot * 0.25) * orbit;
                    camera.position.set(
                        Math.sin(angle) * dist,
                        camY,
                        Math.cos(angle) * dist
                    );
                    camera.lookAt(0, 0, 0);
                }
            },
            resize: function(w, h) {
                camera.aspect = w / h;
                camera.updateProjectionMatrix();
            },
            dispose: function() {
                placeholderGeom.dispose();
                goldMaterial.dispose();
                cubeRenderTarget.dispose();
            }
        };
    }

    return { INPUTS: INPUTS, create: create };
})
