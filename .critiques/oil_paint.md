## 2026-05-06
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: Glacial Crystal Cave (prior orphan attempt was 3D Lava Impasto — warm volcanic; this is cold arctic — opposite temperature axis)
**Critique:**
1. Reference fidelity: Ice stalactites + floor crystals in a cave is a coherent standalone environment replacing the inputImage-dependent Kuwahara filter.
2. Compositional craft: Camera below stalactites looking up creates a dramatic environmental wide shot; cell-repeated crystals fill the frame with density.
3. Technical execution: sdRoundCone for tapered stalactites and floor crystals, domain repeat in XZ for many crystals, 64-step march, cold dual-light system.
4. Liveness: Crystal heights/radii animated with sin(TIME); camera orbits with TIME*orbitSpeed; audioBass modulates brightness.
5. Differentiation: Prior orphan was warm lava (volcanic/fire palette); this is cold arctic ice (navy/cyan/white-cold specular). Different primitive vocabulary (round cones vs displaced plane). Different camera angle (looking up vs angled down).
**Changes:**
- Full rewrite from 2-pass Kuwahara+inputImage to standalone 3D ice cave
- sdRoundCone stalactites from ceiling, floor crystals pointing up
- Cell domain-repeat (density parameter controls cell size)
- Cold dual lighting: white key from above, cyan rim from below
- Palette: midnight navy, glacier blue (user iceColor), white-cold specular 3.0
- fwidth() edge ink darkening on silhouette
- Volumetric fog toward far distance
**HDR peaks reached:** cold specular highlights 3.0, ice surface 2.5, rim light 1.5, ambient fog 0.4
**Estimated rating:** 4.0★
