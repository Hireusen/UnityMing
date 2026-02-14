public enum EBlock : byte
{
    None = 0,
    // Seopluo Core
    CoreShard,
    CoreFoundation,
    CoreNucleus,

    // Seopulo Drill
    MechanicalDrill, // 기계식 드릴
    PneumaticDrill, // 공압 드릴
    LaserDrill, // 레이저 드릴
    AirblastDrill, // 압축 공기분사 드릴
    Cultivator, // 온실
    WaterExtractor, // 물 추출기
    OilExtractor, // 석유 추출기

    // Seopulo Pipe
    Conduit, // 파이프
    PulseConduit, // 펄스 파이프
    PlatedConduit, // 도금된 파이프
    LiquidRouter, // 액체 분배기
    LiquidContainer, // 액체 컨테이너
    LiquidTank, // 액체 탱크
    LiquidJunction, // 액체 교차기
    BridgeConduit, // 다리 파이프
    PhaseConduit, // 위상 파이프

    // Seopulo Turret
    Duo, // 듀오
    Scatter, // 스캐터
    Scorch, // 스코치
    Hail, // 헤일
    Wave, // 파도
    Lancer, // 랜서
    Arc, // 아크
    Parallax, // 패럴랙스
    Swarmer, // 스웜
    Salvo, // 살보
    Segment, // 세그먼트
    Tsunami, // 쓰나미
    Fuse, // 퓨즈
    Ripple, // 립플
    Cyclone, // 사이클론
    Foreshadow, // 포어쉐도우
    Spectre, // 스펙터
    Meltdown, // 멜트다운

    // Seopulo Factory
    GraphitePress, // 흑연 압축기
    MultiPress, // 다중 압축기
    SiliconSmelter, // 실리콘 제련소
    SiliconCrucible, // 실리콘 도가니
    Kiln, // 가마
    PlastaniumCompressor, // 플라스터늄 압축기
    PhaseWeaver, // 메타 합성기
    AlloySmelter, // 설금 제련소
    CryofluidMixer, // 냉각수 혼합기
    PyratiteMixer, // 파이라타이트 혼합기
    BlastMixer, // 폭발물 혼합기
    Melter, // 융해기
    Separator, // 광재 분리기
    Disassembler, // 광재 분해기
    SporePress, // 포자 압축기
    Pulverizer, // 분쇄기 
    CoalCentrifuge, // 석탄 정제기
    Incinerator, // 소각로

    // Seopulo Wall
    CopperWall, // 구리 벽
    LargeCopperWall, // 대형 구리 벽
    TitaniumWall, // 티타늄 벽
    LargeTitaniumWall, // 대형 티타늄 벽
    PlastaniumWall, // 플라스터늄 벽
    LargePlastaniumWall, // 대형 플라스터늄 벽
    ThoriumWall, // 토륨 벽
    LargeThoriumWall, // 대형 토륨 벽
    PhaseWall, // 위상 벽
    LargePhaseWall, // 대형 위상 벽
    SurgeWall, // 설금 벽
    LargeSurgeWall, // 대형 설금 벽
    Door, // 문
    LargeDoor, // 대형 문
    ScrapWall, // 조각 벽
    LargeScrapWall, // 대형 조각 벽
    HugeScrapWall, // 거대한 조각 벽
    GiganticScrapWall, // 엄청나게 큰 조각 벽
    Thruster, // 쓰러스터
}
