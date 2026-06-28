/*{
  "CATEGORIES": [
    "3D",
    "Generator",
    "Audio Reactive",
    "Sports"
  ],
  "DESCRIPTION": "World Cup neon 3D \u2014 all 48 national crests as extruded neon pixel-slabs on a black void with a synthwave grid floor and reflection. Auto-cycles every team, or pick two for a head-to-head VERSUS. Audio reactive.",
  "INPUTS": [
    {
      "NAME": "uMode",
      "LABEL": "Mode",
      "TYPE": "long",
      "VALUES": [
        0,
        1
      ],
      "LABELS": [
        "Cycle All",
        "Versus"
      ],
      "DEFAULT": 0
    },
    {
      "NAME": "uTeamA",
      "LABEL": "Team A (left)",
      "TYPE": "long",
      "VALUES": [
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        13,
        14,
        15,
        16,
        17,
        18,
        19,
        20,
        21,
        22,
        23,
        24,
        25,
        26,
        27,
        28,
        29,
        30,
        31,
        32,
        33,
        34,
        35,
        36,
        37,
        38,
        39,
        40,
        41,
        42,
        43,
        44,
        45,
        46,
        47
      ],
      "LABELS": [
        "Algeria",
        "Argentina",
        "Australia",
        "Austria",
        "Belgium",
        "Bosnia",
        "Brazil",
        "Cabo Verde",
        "Canada",
        "Colombia",
        "Cote dIvoire",
        "Croatia",
        "Curacao",
        "Czechia",
        "DR Congo",
        "Ecuador",
        "Egypt",
        "England",
        "France",
        "Germany",
        "Ghana",
        "Haiti",
        "Iran",
        "Iraq",
        "Japan",
        "Jordan",
        "Mexico",
        "Morocco",
        "Netherlands",
        "New Zealand",
        "Norway",
        "Panama",
        "Paraguay",
        "Portugal",
        "Qatar",
        "Saudi Arabia",
        "Scotland",
        "Senegal",
        "South Africa",
        "South Korea",
        "Spain",
        "Sweden",
        "Switzerland",
        "Tunisia",
        "Turkiye",
        "USA",
        "Uruguay",
        "Uzbekistan"
      ],
      "DEFAULT": 6
    },
    {
      "NAME": "uTeamB",
      "LABEL": "Team B (right)",
      "TYPE": "long",
      "VALUES": [
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        13,
        14,
        15,
        16,
        17,
        18,
        19,
        20,
        21,
        22,
        23,
        24,
        25,
        26,
        27,
        28,
        29,
        30,
        31,
        32,
        33,
        34,
        35,
        36,
        37,
        38,
        39,
        40,
        41,
        42,
        43,
        44,
        45,
        46,
        47
      ],
      "LABELS": [
        "Algeria",
        "Argentina",
        "Australia",
        "Austria",
        "Belgium",
        "Bosnia",
        "Brazil",
        "Cabo Verde",
        "Canada",
        "Colombia",
        "Cote dIvoire",
        "Croatia",
        "Curacao",
        "Czechia",
        "DR Congo",
        "Ecuador",
        "Egypt",
        "England",
        "France",
        "Germany",
        "Ghana",
        "Haiti",
        "Iran",
        "Iraq",
        "Japan",
        "Jordan",
        "Mexico",
        "Morocco",
        "Netherlands",
        "New Zealand",
        "Norway",
        "Panama",
        "Paraguay",
        "Portugal",
        "Qatar",
        "Saudi Arabia",
        "Scotland",
        "Senegal",
        "South Africa",
        "South Korea",
        "Spain",
        "Sweden",
        "Switzerland",
        "Tunisia",
        "Turkiye",
        "USA",
        "Uruguay",
        "Uzbekistan"
      ],
      "DEFAULT": 1
    },
    {
      "NAME": "uHoldTime",
      "LABEL": "Seconds / Team",
      "TYPE": "float",
      "MIN": 0.6,
      "MAX": 8.0,
      "DEFAULT": 2.6
    },
    {
      "NAME": "uSpinSpeed",
      "LABEL": "Spin Speed",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 0.5
    },
    {
      "NAME": "uSpinAmt",
      "LABEL": "Spin Amount",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.2,
      "DEFAULT": 0.32
    },
    {
      "NAME": "uExtrude",
      "LABEL": "Extrude Depth",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 0.6,
      "DEFAULT": 0.26
    },
    {
      "NAME": "uNeon",
      "LABEL": "Neon Intensity",
      "TYPE": "float",
      "MIN": 0.4,
      "MAX": 4.0,
      "DEFAULT": 1.7
    },
    {
      "NAME": "uGlowSize",
      "LABEL": "Glow Size",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 1.0
    },
    {
      "NAME": "uShowScore",
      "LABEL": "Show Score",
      "TYPE": "long",
      "VALUES": [
        0,
        1
      ],
      "LABELS": [
        "Off",
        "On"
      ],
      "DEFAULT": 1
    },
    {
      "NAME": "uScoreA",
      "LABEL": "Score A (left)",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 20,
      "DEFAULT": 0
    },
    {
      "NAME": "uScoreB",
      "LABEL": "Score B (right)",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 20,
      "DEFAULT": 0
    },
    {
      "NAME": "uGoal",
      "LABEL": "GOAL! (trigger anim)",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.0
    },
    {
      "NAME": "uFlicker",
      "LABEL": "Neon Flicker",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.35
    },
    {
      "NAME": "uRemoveBg",
      "LABEL": "Remove Background",
      "TYPE": "long",
      "VALUES": [
        0,
        1
      ],
      "LABELS": [
        "Off",
        "On"
      ],
      "DEFAULT": 0
    },
    {
      "NAME": "uFloorOn",
      "LABEL": "Floor Reflection",
      "TYPE": "long",
      "VALUES": [
        0,
        1
      ],
      "LABELS": [
        "Off",
        "On"
      ],
      "DEFAULT": 1
    },
    {
      "NAME": "uGridOn",
      "LABEL": "Grid Floor",
      "TYPE": "long",
      "VALUES": [
        0,
        1
      ],
      "LABELS": [
        "Off",
        "On"
      ],
      "DEFAULT": 1
    },
    {
      "NAME": "uZoom",
      "LABEL": "Camera Zoom",
      "TYPE": "float",
      "MIN": 0.35,
      "MAX": 2.2,
      "DEFAULT": 1.0
    },
    {
      "NAME": "uCamHeight",
      "LABEL": "Camera Height",
      "TYPE": "float",
      "MIN": -0.4,
      "MAX": 0.6,
      "DEFAULT": 0.06
    },
    {
      "NAME": "audioReact",
      "LABEL": "Audio React",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 1.0
    },
    {
      "NAME": "uVignette",
      "LABEL": "Vignette",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.5
    },
    {
      "NAME": "uScanline",
      "LABEL": "Scanlines",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.3
    }
  ]
}*/

// AUTO-GENERATED — 48 World Cup team crests, 32x32 bitmaps as per-row 32-bit masks.
#define NTEAM 48
// Row masks: bit x set => black pixel. Indexed LOGO[team*32 + y].
const uint LOGO[1536] = uint[1536](
  0u,0u,128u,32764u,32864u,81916u,147504u,845979644u,
  83959864u,929972280u,354455672u,355414128u,26416u,3968u,2629568u,1056704u,
  36249540u,16879500u,29460236u,15777816u,7864320u,4179712u,2092928u,1040384u,
  0u,2640384u,2689024u,2771520u,3001280u,155744u,0u,0u,
  0u,0u,21054080u,8421632u,21054080u,0u,33554304u,134217696u,
  0u,134217696u,33554304u,29364096u,31473536u,33505152u,31406976u,28063104u,
  20194944u,20391552u,17244288u,20391552u,154609296u,423044760u,33505152u,83836704u,
  209460784u,20970624u,51376320u,4440576u,13112064u,1824768u,0u,0u,
  0u,0u,983040u,2038784u,10436352u,27164544u,52330432u,121536480u,
  238975472u,205289584u,477920312u,477903896u,1022901772u,1022893836u,1072701316u,1071648704u,
  1069549536u,1065354224u,520094204u,520444u,2096248u,33553976u,268435200u,268435328u,
  133173120u,58728320u,65408u,8388352u,4193280u,1044480u,0u,0u,
  0u,0u,8117760u,4739584u,7983616u,4739584u,7888384u,0u,
  0u,65013696u,66162624u,66707392u,1677312u,66961344u,67108800u,1579008u,
  66838464u,66838464u,1579008u,66838464u,66961344u,64912320u,62913984u,58093248u,
  49133376u,47962944u,14408448u,6142464u,946176u,245760u,0u,0u,
  0u,0u,147456u,368640u,946176u,0u,516096u,0u,
  16776960u,8410368u,8410368u,8410368u,8410368u,8869120u,9393408u,10441984u,
  12539136u,16733440u,16733440u,16733440u,16733440u,16733440u,16733440u,8345088u,
  4150272u,10443008u,5198336u,11003136u,5245440u,2872320u,0u,0u,
  0u,0u,1044480u,4193280u,15732480u,29606784u,51376320u,102758496u,
  203421744u,214156080u,434111384u,433061272u,868220636u,868220620u,868220620u,931133932u,
  931133932u,868219084u,813686796u,822067212u,406815512u,434048920u,233578416u,217456432u,
  108543584u,52181184u,29361024u,16260864u,4193280u,1044480u,0u,0u,
  0u,0u,147456u,9537792u,6391296u,610420260u,412092696u,403169304u,
  605976612u,8289792u,31459200u,133173216u,62397888u,26745216u,25166208u,29089152u,
  672768u,3916800u,25838976u,25943424u,25166208u,26745216u,28843392u,32509824u,
  14681856u,8289792u,4094976u,946176u,245760u,98304u,0u,0u,
  0u,0u,67108800u,67108800u,67108800u,51130560u,66043584u,59750592u,
  65814208u,67108800u,58773440u,58663104u,66701504u,54520000u,50853056u,50592448u,
  52314048u,50804928u,50589888u,50520256u,50479296u,50479296u,58868160u,63062976u,
  65161152u,32657280u,16408320u,4094976u,1044480u,98304u,0u,0u,
  0u,0u,98304u,1148928u,1824768u,2095104u,69204000u,505411704u,
  522189048u,530577912u,1071643644u,1073741820u,536870904u,268435440u,0u,149197432u,
  355120392u,489608968u,350918008u,0u,434112408u,485494584u,241690224u,252702960u,
  119535840u,59765184u,25682304u,0u,245760u,516096u,0u,0u,
  0u,0u,1044480u,4193280u,16260864u,29459328u,52181184u,108953184u,
  218005296u,234782640u,436109208u,418914072u,853641036u,805822476u,866122188u,132119520u,
  132119520u,869266380u,869266380u,1005582284u,434628504u,434112408u,214954800u,203421744u,
  106953312u,51376320u,29606784u,15732480u,4094976u,946176u,0u,0u,
  0u,0u,67108800u,67108800u,67107264u,67108544u,67108544u,66617024u,
  66063552u,65012160u,54771648u,55041984u,55041984u,58965952u,54558656u,54534080u,
  56674752u,66066624u,66589888u,64757952u,65798336u,63987904u,62964160u,63144384u,
  63308736u,32245632u,33296256u,8134144u,1044480u,98304u,0u,0u,
  0u,0u,4193280u,7872000u,16408320u,33185664u,260821488u,260723184u,
  260723184u,133701600u,75128352u,75161120u,41639488u,66740160u,63443904u,30135168u,
  29905792u,33283968u,12197120u,12311808u,16260864u,15456000u,7091712u,2626560u,
  3148800u,1050624u,1050624u,528384u,270336u,245760u,0u,0u,
  0u,0u,1044480u,4193280u,15732480u,29361024u,50847936u,102242400u,
  203523120u,205668912u,411189528u,419528856u,822083724u,843054156u,849347916u,841557068u,
  841856076u,849945932u,842003532u,824181900u,421525656u,412621080u,206565936u,203695152u,
  101959776u,50331840u,30405504u,16776960u,4193280u,1044480u,0u,0u,
  0u,0u,33554304u,16777344u,134217696u,100319136u,100433824u,100188064u,
  98048928u,94910880u,91517088u,95684512u,91492256u,97795488u,95683488u,95165856u,
  98043808u,94373792u,94693280u,98357152u,100192160u,100254624u,49837888u,24968832u,
  12451072u,6289920u,2343936u,1579008u,417792u,245760u,0u,0u,
  0u,0u,1566720u,2095104u,10066176u,31456128u,65959872u,32503680u,
  32780160u,33185664u,32135040u,31308672u,29261184u,58302144u,49803072u,116291424u,
  115071840u,114715488u,114715488u,115071840u,116291424u,125300448u,62496192u,67010496u,
  33554304u,16776960u,4193280u,1044480u,245760u,98304u,0u,0u,
  0u,0u,33554304u,17891456u,33554304u,134217696u,134217696u,104856672u,
  119535840u,126628320u,130823136u,114242400u,105853536u,122630880u,130622432u,130810848u,
  105779808u,105853536u,105853536u,105853536u,105853536u,105656928u,108805728u,54525120u,
  60815808u,15197952u,16359168u,4094976u,1044480u,98304u,0u,0u,
  0u,0u,1044480u,4193280u,15732480u,29361024u,50395328u,101414240u,
  202769072u,204210544u,475952056u,482278232u,1031173036u,1055654220u,1037697676u,1001747724u,
  929757708u,1054168076u,1034594316u,995446796u,536870904u,402653208u,201326640u,201326640u,
  100663392u,50331840u,29361024u,15732480u,4193280u,1044480u,0u,0u,
  0u,0u,268435440u,167673744u,159280016u,260039664u,251664368u,251660400u,
  258992112u,254013680u,268435440u,167673744u,159280016u,260039664u,251664368u,251660400u,
  258992112u,254013680u,268435440u,167673744u,159280016u,260039664u,251664368u,117442784u,
  124774368u,19394944u,33456000u,8289792u,1044480u,98304u,0u,0u,
  0u,0u,675840u,270336u,675840u,0u,1024u,11264u,
  16128u,32512512u,60309376u,58474240u,48643584u,31407872u,29360000u,24772480u,
  24641408u,7339776u,6290944u,6290432u,2095104u,1863680u,532480u,532480u,
  931840u,0u,1677312u,559104u,1677312u,559104u,0u,0u,
  0u,0u,1044480u,4193280u,15732480u,29877120u,52181184u,108007008u,
  213910320u,226722224u,419545240u,453083352u,302886984u,907917420u,874510380u,875557932u,
  878176812u,875287596u,911583852u,304862280u,453230808u,419578008u,226861488u,216008496u,
  3148800u,60570048u,29877120u,15732480u,3664896u,516096u,0u,0u,
  0u,0u,67043328u,34144256u,36241408u,34144256u,63504384u,38338560u,
  19169280u,9584640u,4792320u,2398208u,1197568u,2175232u,5033088u,5378688u,
  10885440u,11410752u,21020832u,22143648u,22143648u,21020832u,11410752u,10885440u,
  5378688u,4768896u,2496768u,1697280u,399360u,122880u,0u,0u,
  0u,0u,1044480u,4193280u,15732480u,29361024u,50331840u,109051488u,
  205521456u,205521456u,406848024u,411041304u,811598348u,810549772u,4719104u,809763340u,
  809632268u,4260352u,809533964u,809517580u,406856216u,404755480u,202377264u,201855024u,
  100933728u,50577600u,29361024u,15732480u,4193280u,1044480u,0u,0u,
  0u,0u,33554304u,16777344u,16777344u,33554304u,33406848u,33456000u,
  33554304u,16777344u,16777344u,33554304u,16777344u,16777344u,17023104u,17404032u,
  18356352u,20454528u,16777344u,21070464u,23316096u,23316096u,21070464u,16777344u,
  20454528u,9967872u,4821504u,2343936u,1579008u,516096u,0u,0u,
  0u,0u,16776960u,8388864u,28915072u,53187776u,41587776u,33554496u,
  50331456u,50331456u,50331456u,50331456u,50331456u,50331456u,33554496u,35684416u,
  53258432u,19856512u,25167232u,8388864u,14678784u,6289920u,7337472u,3142656u,
  3664896u,1824768u,897024u,417792u,245760u,98304u,0u,0u,
  0u,0u,268435440u,268435440u,202350576u,267792368u,236695536u,268285936u,
  252099568u,268401648u,260176880u,268434416u,267057136u,265609456u,262843376u,257314352u,
  268365360u,267969072u,268435440u,265037808u,263696368u,260452336u,263695856u,129475040u,
  134217696u,33554304u,33554304u,8388096u,1044480u,98304u,0u,0u,
  0u,0u,268435440u,148670640u,178956976u,176868016u,179087504u,268435440u,
  0u,184549200u,184549200u,184549200u,184549200u,184549200u,184450896u,183652176u,
  184033104u,184033104u,183504720u,184303440u,184401744u,192937680u,230686128u,114291552u,
  57921216u,27113856u,15099648u,3677184u,946176u,98304u,0u,0u,
  0u,0u,2097152u,6291456u,15728640u,16244736u,16259072u,24888832u,
  29110528u,47975552u,57549440u,28734016u,47937856u,57670944u,95936672u,98492576u,
  99467424u,91996320u,79151392u,42942784u,39400000u,21209728u,19039360u,530810360u,
  0u,518663816u,304693976u,304647848u,304693896u,518663816u,0u,0u,
  0u,0u,1044480u,3677184u,14681856u,25166208u,50331840u,34070592u,
  34500672u,102512736u,69207072u,70626336u,70405152u,71031840u,70786080u,69056544u,
  101560416u,34598976u,51032256u,25866624u,15382272u,3849216u,700416u,700416u,
  700416u,700416u,716800u,1032192u,196608u,196608u,0u,0u,
  0u,0u,2095104u,7343616u,30405504u,54525120u,117342048u,83672864u,
  98311072u,95173024u,91517088u,95684512u,91492256u,99892640u,99877792u,99360160u,
  98043808u,95422368u,94693280u,98357152u,100192160u,100254624u,100169632u,75300384u,
  125697760u,18872448u,29606784u,7872000u,946176u,245760u,0u,0u,
  0u,0u,348160u,4150272u,16439808u,33483392u,51768000u,101350080u,
  201629392u,73680u,28632u,268438008u,805319672u,805308280u,821038200u,465572348u,
  520619916u,510660108u,535035916u,529006604u,531824644u,535183360u,268206096u,226305072u,
  90130784u,22926016u,22806400u,5963520u,1506304u,421888u,0u,0u,
  0u,0u,67108800u,262142448u,187593936u,220051632u,184918224u,136214544u,
  0u,268287984u,268287984u,268287984u,268287984u,268287984u,98304u,268435440u,
  268435440u,98304u,268287984u,268287984u,268287984u,268287984u,268287984u,134070240u,
  134070240u,33406848u,33406848u,8240640u,1044480u,98304u,0u,0u,
  0u,0u,76706416u,183850320u,177558864u,244799344u,177558800u,177558800u,
  0u,264340464u,132564960u,32739200u,8101376u,268435440u,133173216u,7343616u,
  267448304u,133214176u,7400960u,267849712u,133500896u,7802368u,268115952u,133971936u,
  1996800u,1044480u,2095104u,3394560u,516096u,946176u,0u,0u,
  0u,0u,1044480u,4193280u,15732480u,29361024u,52283584u,101012576u,
  235789424u,218387632u,444596568u,423723672u,813793548u,807401484u,806350860u,805822476u,
  805552140u,805822476u,806252556u,940052492u,404751384u,407898648u,211813680u,222298800u,
  109052256u,50331840u,29361024u,15732480u,4193280u,1044480u,0u,0u,
  0u,0u,8388096u,1579008u,626688u,626688u,1044480u,7872000u,
  276971784u,287161480u,425723544u,507609720u,323060424u,287409288u,295699848u,279819528u,
  279819528u,397260264u,480249144u,413238552u,279020808u,280071432u,8917248u,6809088u,
  1050624u,1044480u,626688u,626688u,1579008u,8388096u,0u,0u,
  0u,0u,1044480u,4193280u,15732480u,29361024u,50331840u,100663392u,
  251658480u,260293104u,533509112u,536027128u,1071745020u,1072732156u,1070068732u,1070745596u,
  1070639100u,1072804860u,1071712252u,1073489916u,535908344u,536657912u,268435440u,267798512u,
  132863968u,65165248u,32196480u,15425280u,4193280u,1044480u,0u,0u,
  0u,0u,1044480u,3849216u,15732480u,29361024u,50331840u,101707872u,
  202378288u,136544784u,407109912u,276824456u,830472652u,569901540u,553910500u,537002004u,
  536870932u,652067316u,575947772u,920503292u,307561464u,441828344u,134218736u,201457968u,
  105121888u,54017216u,29491072u,15859456u,3996672u,1044480u,0u,0u,
  0u,0u,1044480u,4193280u,15732480u,29361024u,51376320u,102758496u,
  218103600u,209715504u,413668632u,310477132u,916669804u,914588012u,916185452u,915112300u,
  915136876u,916948332u,914588012u,914522476u,910873196u,321062088u,428870040u,214504240u,
  107226720u,53726400u,25694592u,14927616u,3677184u,1044480u,0u,0u,
  0u,0u,1044480u,4193280u,16260864u,29606784u,52181184u,102860384u,
  208552240u,201344944u,410031128u,402761432u,843261324u,809570924u,822167884u,817964204u,
  824119852u,837028396u,805573164u,940118604u,403835480u,405017240u,205791920u,203696432u,
  106177120u,50995392u,29688704u,15732480u,4193280u,1044480u,0u,0u,
  0u,0u,33554304u,33554304u,33554304u,32509824u,33025920u,17293440u,
  25067136u,8142336u,33038208u,33554304u,30559104u,30559104u,29361024u,13634432u,
  15210240u,16002816u,83369504u,243170928u,523390200u,261896688u,130970592u,65529792u,
  32755584u,66691008u,33308544u,8388096u,2095104u,245760u,0u,0u,
  0u,0u,8388096u,4194816u,4293120u,4612608u,4821504u,4612608u,
  1148928u,2196480u,4993536u,10634496u,22170240u,20972160u,11261184u,5343744u,
  2245632u,946176u,4723200u,4342272u,4293120u,4194816u,8388096u,0u,
  0u,1287168u,2659328u,3771392u,2659328u,2663424u,0u,0u,
  0u,0u,147456u,98304u,98304u,147456u,0u,33554304u,
  66740160u,65812416u,57945792u,37379136u,58572480u,56772288u,57669312u,57398976u,
  57571008u,57521856u,57521856u,57571008u,57669312u,36949056u,66863040u,37747776u,
  67010496u,33406848u,33406848u,8289792u,1044480u,98304u,0u,0u,
  0u,0u,245760u,417792u,897024u,897024u,3044352u,5761536u,
  27940224u,25165440u,25165440u,16777344u,25067136u,25067136u,25067136u,25067136u,
  16777344u,16777344u,25067136u,25067136u,25067136u,29261184u,14580480u,7239168u,
  3566592u,1726464u,798720u,417792u,245760u,98304u,0u,0u,
  0u,0u,0u,50331648u,75497472u,138412032u,71237632u,50364416u,
  69191648u,135266448u,269549192u,570951812u,688155572u,276957332u,5320412u,2172928u,
  2238464u,4456448u,4898400u,9667088u,19577808u,38175248u,42080992u,419498496u,
  570476928u,603979776u,402653184u,0u,0u,0u,0u,0u,
  0u,0u,1044480u,4193280u,15732480u,29877120u,51910848u,102990944u,
  205734448u,209969456u,419659928u,419659928u,1073741820u,537387012u,1073225724u,806602764u,
  1071933436u,943731228u,1069890556u,868367308u,298138504u,415249176u,140797456u,204721200u,
  102242400u,51376320u,29361024u,15732480u,4193280u,1044480u,0u,0u,
  0u,0u,1044480u,4193280u,16776960u,33554304u,67108800u,134217696u,
  268435440u,268305392u,536732152u,536869112u,1073740924u,1069546556u,1069546556u,1007156284u,
  1059060796u,1041235004u,1055915068u,1073740924u,536869112u,536732152u,268305392u,268435440u,
  134217696u,67108800u,33554304u,16776960u,4193280u,1044480u,0u,0u,
  0u,0u,134217696u,68956960u,80220960u,80319264u,71930656u,68129568u,
  76427808u,80506976u,130675168u,130651104u,100245408u,83468064u,79445280u,77494560u,
  77064480u,76794144u,76695840u,76695840u,110250336u,59918784u,26364288u,13781760u,
  7491072u,3296256u,1726464u,946176u,417792u,245760u,0u,0u,
  0u,0u,21648000u,8659200u,21648000u,0u,67108800u,33554496u,
  36168768u,39217728u,36021312u,39168576u,36168768u,39168576u,35923008u,39316032u,
  35923008u,36152384u,33955904u,34054208u,33554496u,41524800u,33701952u,41266752u,
  17145984u,20073600u,8487168u,7343616u,946176u,98304u,0u,0u,
  0u,0u,98304u,516096u,2095104u,8388096u,33554304u,134217696u,
  528756216u,1069002236u,1069002236u,1065758204u,1069002236u,532131320u,532128248u,536870904u,
  268435440u,268189680u,133947360u,133591008u,65910720u,65689536u,32135040u,15578880u,
  7761408u,3922944u,1849344u,1044480u,516096u,98304u,0u,0u
);
const vec3 TEAMCOL[48] = vec3[48](
  vec3(0.157,0.863,0.471),
  vec3(0.431,0.784,1.000),
  vec3(1.000,0.824,0.157),
  vec3(1.000,0.275,0.353),
  vec3(1.000,0.804,0.000),
  vec3(1.000,0.824,0.157),
  vec3(0.235,0.922,0.510),
  vec3(0.235,0.549,1.000),
  vec3(1.000,0.275,0.353),
  vec3(1.000,0.882,0.157),
  vec3(1.000,0.549,0.157),
  vec3(1.000,0.275,0.353),
  vec3(0.235,0.549,1.000),
  vec3(0.353,0.588,1.000),
  vec3(0.353,0.784,1.000),
  vec3(1.000,0.824,0.157),
  vec3(1.000,0.275,0.353),
  vec3(0.353,0.588,1.000),
  vec3(0.353,0.588,1.000),
  vec3(1.000,0.824,0.157),
  vec3(1.000,0.824,0.157),
  vec3(0.353,0.588,1.000),
  vec3(0.235,0.922,0.510),
  vec3(0.235,0.922,0.510),
  vec3(1.000,0.275,0.431),
  vec3(1.000,0.275,0.353),
  vec3(0.235,0.922,0.510),
  vec3(1.000,0.275,0.353),
  vec3(1.000,0.549,0.157),
  vec3(0.784,0.863,1.000),
  vec3(0.353,0.588,1.000),
  vec3(0.353,0.588,1.000),
  vec3(1.000,0.275,0.353),
  vec3(0.235,0.922,0.510),
  vec3(0.667,0.157,0.353),
  vec3(0.235,0.922,0.510),
  vec3(0.353,0.588,1.000),
  vec3(0.235,0.922,0.510),
  vec3(0.235,0.922,0.510),
  vec3(0.353,0.588,1.000),
  vec3(1.000,0.824,0.157),
  vec3(1.000,0.824,0.157),
  vec3(1.000,0.275,0.353),
  vec3(1.000,0.275,0.353),
  vec3(1.000,0.275,0.353),
  vec3(0.353,0.588,1.000),
  vec3(0.353,0.706,1.000),
  vec3(0.235,0.922,0.510)
);

// ════════════════════════════════════════════════════════════════════════
//  WORLD CUP — NEON 3D CRESTS
//  48 national crests as extruded neon pixel-slabs on a black void with a
//  synthwave grid floor + reflection. Auto-cycles through every team, or
//  pick two for a head-to-head VERSUS face-off. Audio reactive.
// ════════════════════════════════════════════════════════════════════════

#define PI  3.14159265359
#define TAU 6.28318530718

// ── globals set in main() ────────────────────────────────────────────────
float gPulse;    // bass-driven pulse 0..~1
float gHigh;     // treble shimmer
float gFlicker;  // neon-sign flicker multiplier (1.0 = steady)

// ── hashes ───────────────────────────────────────────────────────────────
float hsh11(float n){ return fract(sin(n*127.1)*43758.5453); }
float hsh21(vec2 p){ return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }

// ── neon-sign flicker: mostly steady, rare dropouts + micro shimmer ──────
float neonFlicker(float t){
    float micro = 0.94 + 0.06*sin(t*42.0);
    float seed  = hsh11(floor(t*22.0));
    float dip   = (seed > 0.90) ? mix(0.30, 0.8, hsh11(floor(t*22.0)+3.0)) : 1.0;
    float buzz  = (hsh11(floor(t*9.0)) > 0.96) ? 0.6 : 1.0;
    return micro*dip*buzz;
}

// ════════ 3x5 PIXEL FONT — 0-9, V(10) S(11) -(12) G(13) O(14) A(15) L(16) !(17)
// each glyph = 5 rows, each row 3 bits (bit2=left col)
const int FROW[90] = int[90](
    7,5,5,5,7,   2,6,2,2,7,   7,1,7,4,7,   7,1,7,1,7,   5,5,7,1,1,
    7,4,7,1,7,   7,4,7,5,7,   7,1,2,2,2,   7,5,7,5,7,   7,5,7,1,7,
    5,5,5,5,2,   7,4,7,1,7,   0,0,7,0,0,
    7,4,5,5,7,   7,5,5,5,7,   2,5,7,5,5,   4,4,4,4,7,   2,2,2,0,2
);
float glyphPix(int g, vec2 uv){          // uv in [-1,1]
    if(abs(uv.x) > 1.0 || abs(uv.y) > 1.0) return 0.0;
    int cx = int((uv.x*0.5 + 0.5)*3.0);
    int cy = int((0.5 - uv.y*0.5)*5.0);
    cx = clamp(cx,0,2); cy = clamp(cy,0,4);
    int row = FROW[g*5 + cy];
    return ((row >> (2-cx)) & 1) == 1 ? 1.0 : 0.0;
}
float glyphAt(vec2 p, vec2 ctr, vec2 hsz, int g){
    return glyphPix(g, (p - ctr)/hsz);
}
float numAt(vec2 p, vec2 ctr, vec2 hsz, int val){   // 1-2 digits, centered
    val = clamp(val, 0, 99);
    if(val < 10) return glyphAt(p, ctr, hsz, val);
    float gw = hsz.x*1.35;
    return max(glyphAt(p, ctr-vec2(gw,0.0), hsz, val/10),
               glyphAt(p, ctr+vec2(gw,0.0), hsz, val%10));
}

// ── crest bitmap lookup ──────────────────────────────────────────────────
// team t, local coords uv in roughly [-1,1]; returns 1.0 on a black crest pixel
float logoBit(int t, vec2 uv){
    vec2 z = uv * 0.92;                       // slight zoom past the padding
    float px = (z.x*0.5 + 0.5) * 32.0;
    float py = (0.5 - z.y*0.5) * 32.0;        // flip Y (bitmap is top-down)
    if(px < 0.0 || px >= 32.0 || py < 0.0 || py >= 32.0) return 0.0;
    int xi = int(px), yi = int(py);
    uint mask = LOGO[t*32 + yi];
    return ((mask >> uint(xi)) & 1u) == 1u ? 1.0 : 0.0;
}

// 4-tap coverage for edge anti-aliasing
float logoCov(int t, vec2 uv, float aa){
    float s = logoBit(t, uv + vec2(-aa,-aa));
    s += logoBit(t, uv + vec2( aa,-aa));
    s += logoBit(t, uv + vec2(-aa, aa));
    s += logoBit(t, uv + vec2( aa, aa));
    return s * 0.25;
}

// ── one extruded neon crest card ─────────────────────────────────────────
// p: screen pt (y up). c: center. s: half-height scale. yaw: rotation.
// returns rgb = neon emissive, a = solid coverage
vec4 card(vec2 p, vec2 c, float s, float yaw, int t, vec3 col){
    vec2 q = (p - c) / s;
    if(abs(q.y) > 1.25 || abs(q.x) > 1.8) return vec4(0.0);

    float sa = sin(yaw), ca = cos(yaw);
    float caa = max(abs(ca), 0.14);
    float t2 = uExtrude * (0.55 + 0.45*gPulse);   // half-thickness, breathes with bass

    const int L = 14;
    float hd = -1.0; float ucoord = 0.0;
    // march depth layers front -> back, first crest hit is the visible surface
    for(int i=0; i<L; i++){
        float f  = float(i)/float(L-1);
        float dz = mix(-t2, t2, f);
        float u  = (q.x - dz*sa)/caa;
        if(logoBit(t, vec2(u, q.y)) > 0.5){ hd = f; ucoord = u; break; }
    }
    if(hd < 0.0) return vec4(0.0);

    float cov = logoCov(t, vec2(ucoord, q.y), 0.018);
    cov = max(cov, 0.4);

    float depthShade = mix(1.0, 0.26, hd);                 // side walls fall off dark
    float sheen      = 0.62 + 0.45*smoothstep(-1.0, 1.0, q.y);

    vec3 e = col * (uNeon * 1.35 * depthShade);
    // white-hot core on the front face
    e += vec3(1.0) * uNeon * 0.55 * depthShade * smoothstep(0.5, 0.0, hd);
    e *= sheen;
    e *= (1.0 + gPulse*0.65 + gHigh*0.25);
    return vec4(e, cov);
}

// ── soft neon halo around a crest silhouette ─────────────────────────────
float halo(vec2 p, vec2 c, float s, int t){
    vec2 q = (p - c)/s;
    if(abs(q.x) > 1.8 || abs(q.y) > 1.8) return 0.0;
    float r = 0.045 + 0.16*uGlowSize;
    float g = 0.0;
    const int K = 10;
    for(int i=0; i<K; i++){
        float a = float(i)/float(K) * TAU;
        vec2 d = vec2(cos(a), sin(a));
        g += logoBit(t, q + d*r);
        g += logoBit(t, q + d*r*0.5);
    }
    return g/float(K*2);
}

// ── GOAL confetti — team-colored bits raining down, neon flicker ─────────
vec3 confetti(vec2 p, float amt, vec3 cA, vec3 cB){
    if(amt <= 0.001) return vec3(0.0);
    vec3 e = vec3(0.0);
    for(int l=0; l<3; l++){
        float fl    = float(l);
        float scale = 16.0 + fl*9.0;
        float speed = 0.55 + 0.30*fl;
        vec2 q = p * scale;
        q.x += sin(p.y*3.0 + fl*2.0 + TIME)*0.6;   // drift sway
        q.y += TIME * speed * scale * 0.16;          // fall downward
        vec2 cell = floor(q);
        vec2 f    = fract(q) - 0.5;
        float h = hsh21(cell + fl*41.0);
        if(h > 0.5){
            float rot = (h-0.5)*6.0 + TIME*4.0*sign(h-0.75);
            float cs = cos(rot), sn = sin(rot);
            vec2 fp = mat2(cs,-sn,sn,cs)*f;
            float piece = step(abs(fp.x),0.20)*step(abs(fp.y),0.34);
            float fk = 0.55 + 0.45*sin(TIME*26.0*h + h*60.0);   // per-bit flicker
            vec3 cc = mix(cA, cB, step(0.5, fract(h*7.3)));
            e += cc * piece * fk * (0.7 + 0.5*fl);
        }
    }
    return e * amt * 1.4;
}

// ── "GOAL!" wordmark (G O A L !) ─────────────────────────────────────────
float goalWord(vec2 uv, vec2 ctr, float sz){
    vec2 gh = vec2(0.5,0.85)*sz;
    float sp = 1.5*sz;
    float m = 0.0;
    m = max(m, glyphAt(uv, ctr+vec2(-2.0*sp,0.0), gh, 13));   // G
    m = max(m, glyphAt(uv, ctr+vec2(-1.0*sp,0.0), gh, 14));   // O
    m = max(m, glyphAt(uv, ctr+vec2( 0.0*sp,0.0), gh, 15));   // A
    m = max(m, glyphAt(uv, ctr+vec2( 1.0*sp,0.0), gh, 16));   // L
    m = max(m, glyphAt(uv, ctr+vec2( 2.0*sp,0.0), gh, 17));   // !
    return m;
}

// ── GOAL celebration — a triggerable looping animation (held = it plays) ─
//   arc per loop: burst-in → confetti rain + shockwave → fade. "GOAL!" punches.
vec3 goalCelebration(vec2 uv, float amt, vec3 cA, vec3 cB){
    if(amt <= 0.001) return vec3(0.0);
    float gp  = fract(TIME*0.42);                               // ~2.4s celebration loop
    float env = smoothstep(0.0,0.05,gp) * (1.0 - smoothstep(0.55,1.0,gp));
    vec3 e = vec3(0.0);
    // confetti rains (keeps a little baseline so it never fully empties)
    e += confetti(uv, amt*max(env,0.30), cA, cB);
    // shockwave ring bursting from the title
    float rr   = length(uv - vec2(0.0,0.30));
    float wave = exp(-pow((rr - gp*1.7)*5.0, 2.0)) * env;
    e += mix(cA,cB,0.5) * wave * 2.0;
    // "GOAL!" punches in (scale-in) and flickers like neon
    float sz   = mix(0.10, 0.075, smoothstep(0.0,0.18,gp));
    float g    = goalWord(uv, vec2(0.0,0.30), sz);
    float fk   = 0.6 + 0.4*sin(TIME*30.0);
    vec3  tcol = mix(vec3(1.0), mix(cA,cB,0.5+0.5*sin(TIME*6.0)), 0.45);
    e += g * tcol * (1.6 + env*1.6) * fk;
    return e * amt;
}

// ── VS + live score between the two crests (versus mode) ─────────────────
vec3 vsOverlay(vec2 uv, float cardY, vec3 cA, vec3 cB){
    vec3 e = vec3(0.0);
    // "VS" in the gap, white-hot, sharing the neon flicker
    vec2 gh = vec2(0.034, 0.058);
    vec2 vc = vec2(0.0, cardY + 0.02);
    float vs = max(glyphAt(uv, vc - vec2(0.044,0.0), gh, 10),
                   glyphAt(uv, vc + vec2(0.044,0.0), gh, 11));
    e += vec3(1.0) * vs * 1.8 * gFlicker;
    // score row below, each number in its team's color
    if(uShowScore > 0.5){
        vec2 sh = vec2(0.028, 0.05);
        vec2 sc = vec2(0.0, cardY - 0.34);
        e += cA * numAt(uv, sc - vec2(0.11,0.0), sh, int(uScoreA)) * 1.7;
        e += cB * numAt(uv, sc + vec2(0.11,0.0), sh, int(uScoreB)) * 1.7;
        e += vec3(0.9) * glyphAt(uv, sc, vec2(0.026,0.05), 12) * 1.2;
    }
    return e;
}

// ── synthwave grid floor ─────────────────────────────────────────────────
vec3 gridFloor(vec2 uv, float horizon, vec3 tint){
    if(uv.y >= horizon) return vec3(0.0);
    float d = horizon - uv.y;
    float persp = 1.0/(d + 0.05);
    float gz = (d*persp)*0.55 - TIME*0.9;            // receding lines scroll toward viewer
    float gx = uv.x*persp*0.85;
    float lz = abs(fract(gz) - 0.5);
    float lx = abs(fract(gx) - 0.5);
    float lineZ = smoothstep(0.49, 0.5, lz);
    float lineX = smoothstep(0.47, 0.5, lx);
    float grid = max(lineZ, lineX);
    float fade = exp(-d*2.2) * smoothstep(0.0, 0.08, d);  // fade at horizon + into distance
    vec3 gcol = mix(vec3(0.20,0.35,0.9), tint, 0.5);
    return gcol * grid * fade * (0.6 + 0.8*gPulse) * 0.9;
}

// ── all crests for the current mode (emissive only) ──────────────────────
vec3 crests(vec2 p, float cardY){
    vec3 e = vec3(0.0);
    if(uMode < 0.5){
        // ── CYCLE ── advance through every team with a hidden edge-on flip
        float hold = max(uHoldTime, 0.4);
        float gt = TIME/hold;
        int idx  = int(mod(floor(gt), float(NTEAM)));
        float ph = fract(gt);
        float yaw = sin(TIME*uSpinSpeed)*uSpinAmt;
        int showIdx = idx;
        float flipFrac = 0.22;
        if(ph > 1.0 - flipFrac){
            float k = (ph - (1.0-flipFrac))/flipFrac;     // 0..1
            float sp = k*PI;
            if(k > 0.5){ showIdx = int(mod(float(idx+1), float(NTEAM))); sp = k*PI - PI; }
            yaw += sp;                                     // half-turn flip, swap hidden at edge-on
        }
        vec3 col = TEAMCOL[showIdx];
        float sc = 0.25 * uZoom;
        vec4 s = card(p, vec2(0.0, cardY), sc, yaw, showIdx, col);
        e += s.rgb;
        e += col * halo(p, vec2(0.0, cardY), sc, showIdx) * uNeon * 1.3;
    } else {
        // ── VERSUS ── two crests angled toward each other
        int tA = int(clamp(uTeamA, 0.0, float(NTEAM-1)));
        int tB = int(clamp(uTeamB, 0.0, float(NTEAM-1)));
        vec3 cA = TEAMCOL[tA], cB = TEAMCOL[tB];
        float off = 0.40 * uZoom;
        float sc  = 0.26 * uZoom;
        float bob = sin(TIME*1.3)*0.02;
        // crests spin/wobble on the Y axis around an inward-facing tilt
        float spin = sin(TIME*uSpinSpeed)*uSpinAmt;
        vec2 pa = vec2(-off, cardY + bob);
        vec2 pb = vec2( off, cardY - bob);
        e += card(p, pa, sc,  0.28 + spin, tA, cA).rgb;
        e += card(p, pb, sc, -0.28 - spin, tB, cB).rgb;
        e += cA * halo(p, pa, sc, tA) * uNeon * 1.15;
        e += cB * halo(p, pb, sc, tB) * uNeon * 1.15;
        // clash glow in the middle
        vec2 m = p - vec2(0.0, cardY);
        float cl = exp(-dot(m,m)*7.0);
        e += mix(cA, cB, 0.5) * cl * (0.45 + gPulse*0.9) * (0.65 + 0.35*sin(TIME*5.0));
    }
    return e * gFlicker;          // whole sign flickers together like neon
}

void main(){
    vec2 R  = RENDERSIZE;
    vec2 uv = (gl_FragCoord.xy - 0.5*R)/R.y;     // centered, y up, aspect by height

    gPulse = clamp(audioBass, 0.0, 1.0) * audioReact;
    gHigh  = clamp(audioHigh, 0.0, 1.0) * audioReact;
    gFlicker = mix(1.0, neonFlicker(TIME), uFlicker);

    float cardY   = 0.10 + uCamHeight;
    float horizon = -0.30 + uCamHeight*0.6;

    // team colors on screen (for tint, confetti, score)
    int idxC = int(mod(floor(TIME/max(uHoldTime,0.4)), float(NTEAM)));
    int iA = int(clamp(uTeamA,0.0,float(NTEAM-1)));
    int iB = int(clamp(uTeamB,0.0,float(NTEAM-1)));
    vec3 colA = (uMode < 0.5) ? TEAMCOL[idxC] : TEAMCOL[iA];
    vec3 colB = (uMode < 0.5) ? TEAMCOL[idxC] : TEAMCOL[iB];
    vec3 tint = mix(colA, colB, 0.5);

    bool bg = (uRemoveBg < 0.5);   // background on unless "remove background"

    // ── background (skipped when background removed) ──
    vec3 col = vec3(0.0);
    if(bg){
        // faint nebula glow behind the crest
        float neb = exp(-length(uv - vec2(0.0, cardY))*1.6);
        col += tint * neb * 0.10 * (0.7 + 0.6*gPulse);
        if(uGridOn > 0.5) col += gridFloor(uv, horizon, tint);
        // floor reflection of the crests
        if(uFloorOn > 0.5 && uv.y < horizon){
            vec2 ruv = vec2(uv.x, 2.0*horizon - uv.y);
            float rf = exp(-(horizon - uv.y)*3.2) * 0.45;
            col += crests(ruv, cardY) * rf;
        }
    }

    // ── crests ──
    col += crests(uv, cardY);

    // ── VS + live score (versus mode) ──
    if(uMode > 0.5) col += vsOverlay(uv, cardY, colA, colB);

    // ── GOAL celebration (triggerable animation) ──
    col += goalCelebration(uv, uGoal, colA, colB);

    // ── post ──
    // soft bloom-ish lift
    col += col*col*0.18;
    // chromatic shimmer on the treble
    // tonemap
    col = col/(1.0+col);
    col = pow(col, vec3(0.85));
    // scanlines
    col *= 1.0 - uScanline*0.12*(0.5+0.5*sin(gl_FragCoord.y*1.4 + TIME*2.0));
    // vignette (only with a background)
    if(uRemoveBg < 0.5){
        float vig = smoothstep(1.45, 0.35, length(uv*vec2(R.x/R.y,1.0)));
        col *= mix(1.0, vig, uVignette);
    }

    // alpha: opaque normally; with background removed, key on luminance so only
    // the neon crests / text / confetti show on a transparent canvas
    float alpha = 1.0;
    if(uRemoveBg > 0.5){
        float lum = max(col.r, max(col.g, col.b));
        alpha = smoothstep(0.015, 0.16, lum);
    }
    gl_FragColor = vec4(col, alpha);
}
