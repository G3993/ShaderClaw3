(function(THREE) {
    var INPUTS = [
        { NAME: "texture", LABEL: "Image", TYPE: "image" },
        { NAME: "shape", LABEL: "Shape", TYPE: "long", DEFAULT: 0, VALUES: [0,1,2,3,4], LABELS: ["Diamond","Prism","Emerald","Obelisk","Cluster"] },
        { NAME: "hueShift", LABEL: "Hue Shift", TYPE: "float", DEFAULT: 0.0, MIN: 0.0, MAX: 1.0 },
        { NAME: "hueSpread", LABEL: "Hue Range", TYPE: "float", DEFAULT: 0.35, MIN: 0.0, MAX: 1.0 },
        { NAME: "glassOpacity", LABEL: "Glass Opacity", TYPE: "float", DEFAULT: 0.55, MIN: 0.1, MAX: 1.0 },
        { NAME: "refraction", LABEL: "Refraction", TYPE: "float", DEFAULT: 0.92, MIN: 0.8, MAX: 0.99 },
        { NAME: "leadWidth", LABEL: "Lead Width", TYPE: "float", DEFAULT: 1.5, MIN: 0.0, MAX: 4.0 },
        { NAME: "imageSize", LABEL: "Image Size", TYPE: "float", DEFAULT: 1.2, MIN: 0.2, MAX: 3.0 },
        { NAME: "imageDepth", LABEL: "Image Depth", TYPE: "float", DEFAULT: 0.0, MIN: -1.0, MAX: 1.0 },
        { NAME: "speed", LABEL: "Speed", TYPE: "float", DEFAULT: 0.3, MIN: 0.0, MAX: 2.0 },
        { NAME: "rotX", LABEL: "Rotate X", TYPE: "float", DEFAULT: 0.0, MIN: -3.14, MAX: 3.14 },
        { NAME: "rotY", LABEL: "Rotate Y", TYPE: "float", DEFAULT: 0.0, MIN: -3.14, MAX: 3.14 },
        { NAME: "rotZ", LABEL: "Rotate Z", TYPE: "float", DEFAULT: 0.0, MIN: -3.14, MAX: 3.14 },
        { NAME: "size", LABEL: "Size", TYPE: "float", DEFAULT: 1.0, MIN: 0.2, MAX: 3.0 },
        { NAME: "movement", LABEL: "Camera Move", TYPE: "float", DEFAULT: 0.4, MIN: 0.0, MAX: 2.0 },
        { NAME: "transparentBg", LABEL: "Transparent BG", TYPE: "bool", DEFAULT: true },
        { NAME: "bgColor", LABEL: "BG Color", TYPE: "color", DEFAULT: [0.015, 0.015, 0.03, 1.0] }
    ];

    // ===== Gem geometries (return arrays of individual face triangles) =====
    // Each returns { faces: [ [a,b,c], ... ], verts: Float32Array, normals: Float32Array }

    function computeFaceNormal(a, b, c) {
        var ab = [b[0]-a[0], b[1]-a[1], b[2]-a[2]];
        var ac = [c[0]-a[0], c[1]-a[1], c[2]-a[2]];
        var n = [
            ab[1]*ac[2] - ab[2]*ac[1],
            ab[2]*ac[0] - ab[0]*ac[2],
            ab[0]*ac[1] - ab[1]*ac[0]
        ];
        var len = Math.sqrt(n[0]*n[0] + n[1]*n[1] + n[2]*n[2]) || 1;
        return [n[0]/len, n[1]/len, n[2]/len];
    }

    function createDiamondFaces() {
        var faces = [];
        var crownH = 0.4, pavilionH = 0.85, girdle = 0.95, table = 0.55;
        var sides = 8;
        for (var i = 0; i < sides; i++) {
            var a0 = (i / sides) * Math.PI * 2;
            var a1 = ((i + 1) / sides) * Math.PI * 2;
            var g0 = [Math.cos(a0)*girdle, 0, Math.sin(a0)*girdle];
            var g1 = [Math.cos(a1)*girdle, 0, Math.sin(a1)*girdle];
            var t0 = [Math.cos(a0)*table, crownH, Math.sin(a0)*table];
            var t1 = [Math.cos(a1)*table, crownH, Math.sin(a1)*table];
            // Crown lower
            faces.push([g0, g1, t1]);
            faces.push([g0, t1, t0]);
            // Table
            faces.push([[0, crownH, 0], t0, t1]);
            // Pavilion
            faces.push([g1, g0, [0, -pavilionH, 0]]);
        }
        return faces;
    }

    function createPrismFaces() {
        var faces = [], sides = 6, r = 0.7, h = 1.4;
        for (var i = 0; i < sides; i++) {
            var a0 = (i/sides)*Math.PI*2, a1 = ((i+1)/sides)*Math.PI*2;
            var bx0=Math.cos(a0)*r, bz0=Math.sin(a0)*r;
            var bx1=Math.cos(a1)*r, bz1=Math.sin(a1)*r;
            faces.push([[bx0,h/2,bz0],[bx1,h/2,bz1],[bx1,-h/2,bz1]]);
            faces.push([[bx0,h/2,bz0],[bx1,-h/2,bz1],[bx0,-h/2,bz0]]);
            faces.push([[0,h/2,0],[bx0,h/2,bz0],[bx1,h/2,bz1]]);
            faces.push([[0,-h/2,0],[bx1,-h/2,bz1],[bx0,-h/2,bz0]]);
        }
        return faces;
    }

    function createEmeraldFaces() {
        var faces = [];
        var pts = [];
        var sx = 0.9, sz = 0.6, bevel = 0.25;
        pts.push([sx, 0, sz-bevel]);
        pts.push([sx-bevel*0.5, 0, sz]);
        pts.push([-sx+bevel*0.5, 0, sz]);
        pts.push([-sx, 0, sz-bevel]);
        pts.push([-sx, 0, -sz+bevel]);
        pts.push([-sx+bevel*0.5, 0, -sz]);
        pts.push([sx-bevel*0.5, 0, -sz]);
        pts.push([sx, 0, -sz+bevel]);
        var h = 0.5, taper = 0.65;
        for (var i = 0; i < pts.length; i++) {
            var a = pts[i], b = pts[(i+1)%pts.length];
            var at = [a[0]*taper, h, a[2]*taper], bt = [b[0]*taper, h, b[2]*taper];
            var ab = [a[0]*taper, -h, a[2]*taper], bb = [b[0]*taper, -h, b[2]*taper];
            faces.push([[a[0],0,a[2]], [b[0],0,b[2]], bt]);
            faces.push([[a[0],0,a[2]], bt, at]);
            faces.push([[0,h,0], at, bt]);
            faces.push([[b[0],0,b[2]], [a[0],0,a[2]], ab]);
            faces.push([[b[0],0,b[2]], ab, bb]);
            faces.push([[0,-h,0], bb, ab]);
        }
        return faces;
    }

    function createObeliskFaces() {
        var faces = [], sides = 6, rBase = 0.5, rMid = 0.45, hBody = 1.0, hTip = 0.6;
        for (var i = 0; i < sides; i++) {
            var a0=(i/sides)*Math.PI*2, a1=((i+1)/sides)*Math.PI*2;
            var bx0=Math.cos(a0), bz0=Math.sin(a0), bx1=Math.cos(a1), bz1=Math.sin(a1);
            faces.push([[bx0*rBase,-hBody,bz0*rBase],[bx1*rBase,-hBody,bz1*rBase],[bx1*rMid,0,bz1*rMid]]);
            faces.push([[bx0*rBase,-hBody,bz0*rBase],[bx1*rMid,0,bz1*rMid],[bx0*rMid,0,bz0*rMid]]);
            faces.push([[bx0*rMid,0,bz0*rMid],[bx1*rMid,0,bz1*rMid],[0,hTip,0]]);
            faces.push([[0,-hBody,0],[bx1*rBase,-hBody,bz1*rBase],[bx0*rBase,-hBody,bz0*rBase]]);
        }
        return faces;
    }

    function getFaces(shapeId) {
        switch(shapeId) {
            case 1: return createPrismFaces();
            case 2: return createEmeraldFaces();
            case 3: return createObeliskFaces();
            default: return createDiamondFaces();
        }
    }

    // ===== HSV helpers =====
    function hsl2rgb(h, s, l) {
        var c = (1 - Math.abs(2*l - 1)) * s;
        var x = c * (1 - Math.abs((h*6) % 2 - 1));
        var m = l - c/2;
        var r, g, b;
        var i = Math.floor(h * 6) % 6;
        if (i===0) { r=c; g=x; b=0; }
        else if (i===1) { r=x; g=c; b=0; }
        else if (i===2) { r=0; g=c; b=x; }
        else if (i===3) { r=0; g=x; b=c; }
        else if (i===4) { r=x; g=0; b=c; }
        else { r=c; g=0; b=x; }
        return [r+m, g+m, b+m];
    }

    function create(renderer, canvas, media) {
        var scene = new THREE.Scene();
        var _bgColor = new THREE.Color(0.015, 0.015, 0.03);
        scene.background = null;

        var camera = new THREE.PerspectiveCamera(45, canvas.width / canvas.height, 0.1, 100);
        camera.position.set(0, 0.5, 3.5);
        camera.lookAt(0, 0, 0);

        // Lighting — clean and minimal
        scene.add(new THREE.AmbientLight(0x404060, 0.3));
        var keyLight = new THREE.DirectionalLight(0xffffff, 0.8);
        keyLight.position.set(3, 4, 5);
        scene.add(keyLight);
        var rimLight = new THREE.DirectionalLight(0x8090ff, 0.3);
        rimLight.position.set(-3, 2, -2);
        scene.add(rimLight);
        var fillLight = new THREE.PointLight(0xffe0c0, 0.2, 15);
        fillLight.position.set(0, -3, 2);
        scene.add(fillLight);

        // CubeCamera for refraction
        var cubeRT = new THREE.WebGLCubeRenderTarget(512, {
            format: THREE.RGBAFormat,
            generateMipmaps: true,
            minFilter: THREE.LinearMipmapLinearFilter
        });
        var cubeCamera = new THREE.CubeCamera(0.01, 20, cubeRT);
        scene.add(cubeCamera);
        cubeRT.texture.mapping = THREE.CubeRefractionMapping;

        var pivot = new THREE.Group();
        scene.add(pivot);

        // Image plane inside the crystal
        var imageMat = new THREE.MeshBasicMaterial({
            side: THREE.DoubleSide,
            transparent: true,
            depthWrite: false
        });
        var imageGeo = new THREE.PlaneGeometry(2, 2);
        var imagePlane = new THREE.Mesh(imageGeo, imageMat);
        imagePlane.visible = false;
        pivot.add(imagePlane);

        // State
        var faceMeshes = [];    // individual glass panels
        var leadLines = null;   // EdgesGeometry wireframe
        var faceMats = [];
        var currentShape = -1;
        var currentTexId = null;
        var _cubeFrame = 0;

        // Lead came material — dark gunmetal
        var leadMat = new THREE.LineBasicMaterial({
            color: new THREE.Color(0.08, 0.08, 0.1),
            linewidth: 1
        });

        function buildCrystal(shapeId, hueShift, hueSpread, opacity, refractionRatio) {
            // Clean up old
            faceMeshes.forEach(function(m) { pivot.remove(m); m.geometry.dispose(); });
            faceMeshes = [];
            faceMats.forEach(function(m) { m.dispose(); });
            faceMats = [];
            if (leadLines) { pivot.remove(leadLines); leadLines.geometry.dispose(); leadLines = null; }

            if (shapeId === 4) {
                // Cluster: build multiple crystals
                var clusterDefs = [
                    { shape: 0, pos: [0, 0.1, 0], rot: [0, 0, 0], s: 0.65 },
                    { shape: 3, pos: [0.55, -0.35, 0.15], rot: [0.2, 0.4, -0.3], s: 0.45 },
                    { shape: 3, pos: [-0.45, -0.25, -0.2], rot: [-0.15, 0.7, 0.2], s: 0.5 },
                    { shape: 1, pos: [0.15, 0.55, -0.15], rot: [0.35, -0.2, 0.15], s: 0.3 },
                    { shape: 3, pos: [-0.25, -0.55, 0.3], rot: [-0.25, 0.15, -0.4], s: 0.38 }
                ];
                for (var ci = 0; ci < clusterDefs.length; ci++) {
                    var def = clusterDefs[ci];
                    var subFaces = getFaces(def.shape);
                    var subGroup = new THREE.Group();
                    subGroup.position.set(def.pos[0], def.pos[1], def.pos[2]);
                    subGroup.rotation.set(def.rot[0], def.rot[1], def.rot[2]);
                    subGroup.scale.setScalar(def.s);

                    buildFacesIntoGroup(subGroup, subFaces, hueShift + ci * 0.12, hueSpread, opacity, refractionRatio);
                    pivot.add(subGroup);
                    // Track for cleanup (store group ref on first mesh)
                    faceMeshes.push(subGroup);
                }
            } else {
                var faces = getFaces(shapeId);
                buildFacesIntoGroup(pivot, faces, hueShift, hueSpread, opacity, refractionRatio);
            }

            // Build wireframe edges for lead cames using a merged geometry
            var mergedVerts = [];
            if (shapeId !== 4) {
                var allFaces = getFaces(shapeId);
                for (var fi = 0; fi < allFaces.length; fi++) {
                    var f = allFaces[fi];
                    mergedVerts.push(f[0][0], f[0][1], f[0][2]);
                    mergedVerts.push(f[1][0], f[1][1], f[1][2]);
                    mergedVerts.push(f[2][0], f[2][1], f[2][2]);
                }
                var mergedGeo = new THREE.BufferGeometry();
                mergedGeo.setAttribute('position', new THREE.Float32BufferAttribute(mergedVerts, 3));
                var edgesGeo = new THREE.EdgesGeometry(mergedGeo, 1);
                leadLines = new THREE.LineSegments(edgesGeo, leadMat);
                pivot.add(leadLines);
                mergedGeo.dispose();
            }

            currentShape = shapeId;
        }

        function buildFacesIntoGroup(group, faces, hueShift, hueSpread, opacity, refractionRatio) {
            for (var fi = 0; fi < faces.length; fi++) {
                var f = faces[fi];
                var n = computeFaceNormal(f[0], f[1], f[2]);

                // Per-face glass material with unique stained-glass hue
                var hue = (hueShift + (fi * 0.618033988749) * hueSpread) % 1.0;
                var rgb = hsl2rgb(hue, 0.5, 0.45);

                var mat = new THREE.MeshPhongMaterial({
                    color: new THREE.Color(rgb[0], rgb[1], rgb[2]),
                    envMap: cubeRT.texture,
                    refractionRatio: refractionRatio,
                    reflectivity: 0.85,
                    shininess: 180,
                    specular: new THREE.Color(0.5, 0.5, 0.55),
                    transparent: true,
                    opacity: opacity,
                    side: THREE.DoubleSide,
                    combine: THREE.MixOperation,
                    flatShading: true
                });
                faceMats.push(mat);

                var geo = new THREE.BufferGeometry();
                var verts = new Float32Array([
                    f[0][0], f[0][1], f[0][2],
                    f[1][0], f[1][1], f[1][2],
                    f[2][0], f[2][1], f[2][2]
                ]);
                var norms = new Float32Array([
                    n[0], n[1], n[2],
                    n[0], n[1], n[2],
                    n[0], n[1], n[2]
                ]);
                // Spherical UV projection so image wraps around crystal
                var uvs = new Float32Array(6);
                for (var vi = 0; vi < 3; vi++) {
                    var px = verts[vi*3], py = verts[vi*3+1], pz = verts[vi*3+2];
                    var len2 = Math.sqrt(px*px + py*py + pz*pz) || 1;
                    var nx2 = px/len2, ny2 = py/len2, nz2 = pz/len2;
                    uvs[vi*2]   = 0.5 + Math.atan2(nz2, nx2) / (2 * Math.PI);
                    uvs[vi*2+1] = 0.5 + Math.asin(Math.max(-1, Math.min(1, ny2))) / Math.PI;
                }
                geo.setAttribute('position', new THREE.BufferAttribute(verts, 3));
                geo.setAttribute('normal', new THREE.BufferAttribute(norms, 3));
                geo.setAttribute('uv', new THREE.BufferAttribute(uvs, 2));
                geo.computeBoundingSphere();

                var mesh = new THREE.Mesh(geo, mat);
                group.add(mesh);
                faceMeshes.push(mesh);
            }
        }

        function setFacesVisible(visible) {
            for (var i = 0; i < faceMeshes.length; i++) {
                faceMeshes[i].visible = visible;
            }
            if (leadLines) leadLines.visible = visible;
        }

        return {
            scene: scene,
            camera: camera,
            update: function(time, values, mediaList) {
                var spd = values.speed != null ? values.speed : 0.3;
                var sz = values.size != null ? values.size : 1.0;
                var rx = values.rotX != null ? values.rotX : 0.0;
                var ry = values.rotY != null ? values.rotY : 0.0;
                var rz = values.rotZ != null ? values.rotZ : 0.0;
                var shapeId = values.shape != null ? values.shape : 0;
                var hueShift = values.hueShift != null ? values.hueShift : 0.0;
                var hueSpread = values.hueSpread != null ? values.hueSpread : 0.35;
                var opacity = values.glassOpacity != null ? values.glassOpacity : 0.55;
                var refractionRatio = values.refraction != null ? values.refraction : 0.92;

                // Rebuild crystal if shape changed
                if (shapeId !== currentShape) {
                    buildCrystal(shapeId, hueShift, hueSpread, opacity, refractionRatio);
                }

                // Background
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

                // Update face materials live
                for (var i = 0; i < faceMats.length; i++) {
                    var mat = faceMats[i];
                    var hue = (hueShift + (i * 0.618033988749) * hueSpread) % 1.0;
                    var rgb = hsl2rgb(hue, 0.5, 0.45);
                    mat.color.setRGB(rgb[0], rgb[1], rgb[2]);
                    mat.opacity = opacity;
                    mat.refractionRatio = refractionRatio;
                }

                // Lead edge width
                var lw = values.leadWidth != null ? values.leadWidth : 1.5;
                leadMat.linewidth = lw;
                if (leadLines) leadLines.visible = lw > 0.05;

                // Transform — slow elegant rotation
                var t = time * spd;
                pivot.rotation.set(
                    rx + Math.sin(t * 0.3) * 0.08,
                    ry + t * 0.15,
                    rz + Math.cos(t * 0.25) * 0.04
                );
                pivot.scale.setScalar(sz);

                // Image/video texture — mapped onto crystal faces + internal plane for refraction
                var texId = values.texture;
                if (texId && mediaList) {
                    var m = mediaList.find(function(e) { return String(e.id) === String(texId); });
                    if (m && m.threeTexture) {
                        if (currentTexId !== texId) {
                            m.threeTexture.wrapS = THREE.ClampToEdgeWrapping;
                            m.threeTexture.wrapT = THREE.ClampToEdgeWrapping;
                            m.threeTexture.minFilter = THREE.LinearFilter;
                            m.threeTexture.needsUpdate = true;
                            // Apply to all crystal face materials
                            for (var ti = 0; ti < faceMats.length; ti++) {
                                faceMats[ti].map = m.threeTexture;
                                faceMats[ti].needsUpdate = true;
                            }
                            // Also on internal plane for CubeCamera refraction capture
                            imageMat.map = m.threeTexture;
                            imageMat.needsUpdate = true;
                            imagePlane.visible = true;
                            currentTexId = texId;
                        }
                        if (m.threeTexture.isVideoTexture) {
                            m.threeTexture.needsUpdate = true;
                        }
                    }
                } else if (!texId && currentTexId) {
                    for (var ti = 0; ti < faceMats.length; ti++) {
                        faceMats[ti].map = null;
                        faceMats[ti].needsUpdate = true;
                    }
                    imageMat.map = null;
                    imageMat.needsUpdate = true;
                    imagePlane.visible = false;
                    currentTexId = null;
                }

                // Position image plane inside crystal, billboard to camera
                var imgSize = values.imageSize != null ? values.imageSize : 1.2;
                var imgDepth = values.imageDepth != null ? values.imageDepth : 0.0;
                imagePlane.scale.setScalar(imgSize);
                imagePlane.position.set(0, 0, imgDepth);
                // Billboard: face camera, undo pivot rotation
                imagePlane.quaternion.copy(camera.quaternion);
                var pivotQ = new THREE.Quaternion();
                pivot.getWorldQuaternion(pivotQ);
                pivotQ.invert();
                imagePlane.quaternion.premultiply(pivotQ);

                // Camera orbit
                var orbit = values.movement != null ? values.movement : 0.4;
                if (orbit > 0.001) {
                    var ot = time * 0.15 * orbit;
                    var angle = ot + 0.25 * Math.sin(ot * 0.6);
                    var dist = 3.2 + 0.35 * Math.sin(ot * 0.35) * orbit;
                    var camY = 0.2 + 0.4 * Math.sin(ot * 0.25) * orbit;
                    camera.position.set(
                        Math.sin(angle) * dist,
                        camY,
                        Math.cos(angle) * dist
                    );
                    camera.lookAt(0, 0, 0);
                }

                // CubeCamera update — hide crystal faces, capture image + lights
                _cubeFrame++;
                if (_cubeFrame % 2 === 0) {
                    setFacesVisible(false);
                    cubeCamera.position.set(0, 0, 0);
                    pivot.localToWorld(cubeCamera.position);
                    cubeCamera.update(renderer, scene);
                    setFacesVisible(true);
                }

                // Subtle light movement
                fillLight.position.set(
                    Math.sin(time * 0.4) * 2,
                    -2 + Math.sin(time * 0.25) * 0.4,
                    2 + Math.cos(time * 0.35)
                );
            },
            resize: function(w, h) {
                camera.aspect = w / h;
                camera.updateProjectionMatrix();
            },
            dispose: function() {
                faceMeshes.forEach(function(m) {
                    if (m.geometry) m.geometry.dispose();
                });
                faceMats.forEach(function(m) { m.dispose(); });
                if (leadLines) leadLines.geometry.dispose();
                leadMat.dispose();
                imageMat.dispose();
                imageGeo.dispose();
                cubeRT.dispose();
            }
        };
    }

    return { INPUTS: INPUTS, create: create };
})
