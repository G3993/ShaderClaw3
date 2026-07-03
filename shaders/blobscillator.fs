/*{
	"CREDIT": "by mojovideotech",
  	"CATEGORIES" : [
  		"generator",
    	"blobs",
    	"distance",
    	"noise"
  ],
  	"DESCRIPTION" : "based on https:\/\/www.shadertoy.com\/view\/MlKXWm by cacheflowe.  A wannabe reaction-diffusion, but not at all :-P ",
  	"ISFVSN" : "2",
	"INPUTS" : [
	{
		"NAME": 	"scale",
		"TYPE": 	"float",
		"DEFAULT": 	3.5,
		"MIN": 		0.0,
		"MAX": 		10
	},
	{
		"NAME": 	"rate",
		"TYPE": 	"float",
		"DEFAULT": 	0.125,
		"MIN": 		0.0,
		"MAX": 		1.0
	},
	{
		"NAME": 	"loops",
		"TYPE": 	"float",
		"DEFAULT":	33.0,
		"MIN": 		1.0,
		"MAX": 		100.0
	},
	{
		"NAME": 	"center",
		"TYPE": 	"point2D",
		"DEFAULT":	[ 0, 0 ],
		"MAX" : 	[ 1.0, 1.0 ],
     	"MIN" : 	[ -1.0, -1.0 ]
	},
	{
		"NAME": 	"freq1",
		"TYPE": 	"float",
		"DEFAULT": 	0.95,
		"MIN": 		0.005,
		"MAX": 		1.0
	},
	{
		"NAME": 	"freq2",
		"TYPE": 	"float",
		"DEFAULT": 	3.0,
		"MIN": 		0.5,
		"MAX": 		10.0
	},
	{
     	"NAME" :	"seed1",
     	"TYPE" : 	"float",
     	"DEFAULT" :	233,
     	"MIN" : 	89,
     	"MAX" :		1597
	},
    {
     	"NAME" :	"seed2",
      	"TYPE" :	"float",
     	"DEFAULT" :	13,
     	"MIN" :		5,
     	"MAX" :		55
    },
    {
     	"NAME" :	"audioReact",
     	"LABEL" :	"Audio React",
      	"TYPE" :	"float",
     	"DEFAULT" :	0.35,
     	"MIN" :		0.0,
     	"MAX" :		2.0
    }
  ]
}
*/

////////////////////////////////////////////////////////////
// Blobscillator  by mojovideotech
//
// based on :
// shadertoy.com\/view\/MlKXWm  
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

float hash (float a) { return floor(cos(a)*seed1+sin(a*seed2));  }

void main() {

    // ── Audio conditioning — soft knees, idle floor = baseline look.
    float bassP = pow(smoothstep(0.05, 0.85, audioBass), 1.6);
    float midP  = smoothstep(0.08, 0.90, audioMid);
    float highP = pow(smoothstep(0.10, 0.90, audioHigh), 1.2);
    float aKick = audioBeatPulse * audioBeatPulse;

    vec2 uv = (2.0 * gl_FragCoord.xy - RENDERSIZE.xy) / RENDERSIZE.y;
    uv -= center.xy;
    // Bass zooms the whole blob field — the dominant structural knob.
    float zoomA = 1.0 - audioReact * 0.20 * bassP;
    uv *= (10.5-scale) * zoomA;
    float C = sin(TIME * rate) * freq1, dist = 0.0;
    // Mids widen the blob-node spread — fine turbulence, not structure.
    float freq2A = freq2 * (1.0 + audioReact * 0.25 * midP);
    for(float i=10.0; i < 90.0; i++) {
        float R = C + i;
        vec2 N = vec2(sin(R), cos(R));
        N *= abs(hash(R)) * freq2A;
        dist += sin(i + loops * distance(uv, N));
    }
    // Beat pulse: a decaying brightness accent, not a continuous pump.
    dist *= 1.0 + audioReact * 0.45 * aKick;
    // Highs add a fine shimmer across the field — sparse, subtle detail.
    dist += audioReact * 0.12 * highP * sin(dist * 3.0 + TIME * 2.0);
	gl_FragColor = vec4(vec3(dist),1.0);
}
