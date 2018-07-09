
-- 档案
drop table archives;
create table archives (
    archive_id      varchar(50)     PRIMARY KEY,
    create_time     varchar(20)     NOT NULL,
    scene_id        varchar(20)     not null,
    play_time       integer         not null,
    power_source    char(1)         not null,
    unlock          char(1)         not null,
    finish          char(1)         not null
);

--玩家
drop table players;
create table players (
    player_id       varchar(50)     primary key,
    health          float           not null,
    weapon          varchar(15)     not null,
    position        varchar(50)     not null,
    rotate          float           not null,
    flashlight      char(1)         not null
);

--玩家的物品
drop table player_items;
create table player_items (
    player_id       varchar(50)     not null,
    item_id         varchar(15)     not null,
    amount          integer         not null,
    primary key(player_id, item_id)
);

-- 场景
drop table scenes;
create table scenes (
    scene_id        varchar(30)     primary key not null,
    scene_name      varchar(30)     not null,
    asset_bundle    varchar(30)     not null,
    bgm             varchar(30)     not null,
    power_source    char(1)          not null
);

-- 物品
drop table items;
create table items(
    item_id             varchar(20)     primary key not null,
    item_name           varchar(20)     not null,
    sprite              varchar(20)     not null,
    prefab              varchar(20)     not null,
    assetbundle         varchar(20)     not null,
    script              varchar(50)     not null
);

-- 场景数据
drop table scene_datas;
create table scene_datas (
    archive_id          varchar(50)     not null,
    scene_id            varchar(20)     not null,
    entry               char(1)         not null,
    primary key(archive_id, scene_id)
);

-- 场景物品数据
drop table scene_item_datas;
create table scene_item_datas (
    archive_id          varchar(50)     not null,
    scene_id            varchar(20)     not null,
    scene_item_id       varchar(30)     not null,
    item_id             varchar(20)     not null,
    pick_up             char(1)         not null,
    primary key(archive_id, scene_id, scene_item_id)
);


insert into scenes values ('FrontPolicementGate', '警局大门前', 'scene/front_policement_gate', 'The Basement Of Police Station.mp3', '0');
insert into scenes values ('FrontHall', '前厅', 'scene/front_hall', 'The Front Hall.mp3', '1');
insert into scenes values ('FirstFloorCorridor', '一楼走廊', 'scene/first_floor_corridor', 'The First Floor.mp3', '1');
insert into scenes values ('SecondFloorCorridor', '二楼走廊', 'scene/second_floor_corridor', 'The Second Floor.mp3', '1');
insert into scenes values ('PowerSourceRoom', '电源控制室', 'scene/power_source_room', 'The First Floor.mp3', '0');
insert into scenes values ('CasinoRoom', '娱乐室', 'scene/casino_room', 'The Library.mp3', '0');
insert into scenes values ('PoliceOffice', '值班室', 'scene/police_office', 'The Library.mp3', '0');
insert into scenes values ('BattleRoom', '作战会议室', 'scene/battle_room', 'Secure Place.mp3', '0');
insert into scenes values ('HelicopterParkingStation', '停机坪', 'scene/helicopter_parking_station', 'Escape From Laboratory.mp3', '0');
insert into scenes values ('OpenDoor', '过渡场景', 'scene/open_door', ' ', '0');
insert into scenes values ('OpenGate', '过渡场景', 'scene/open_gate', ' ', '0');


insert into items values ('Flashlight', '手电筒', 'Flashlight', 'Flashlight_prefab', 'flashlight', 'MyResidentEvil.FlashlightItemController');
insert into items values ('HandGun', '手枪', 'HandGun', 'HandGun_prefab', 'handgun', 'MyResidentEvil.WeaponItemController');
insert into items values ('Key_Gold', '金色钥匙', 'Key_Gold', 'key_gold', 'keys', 'MyResidentEvil.DummyItemController');
insert into items values ('Key_Silver', '银色钥匙', 'Key_Silver', 'key_silver', 'keys', 'MyResidentEvil.DummyItemController');
insert into items values ('MechineGun', '重机枪', 'MechineGun', 'MachineGun_prefab', 'machinegun', 'MyResidentEvil.WeaponItemController');
insert into items values ('MedicalBox', '医疗箱', 'MedicalBox', 'MedicalBox_prefab', 'medicalbox', 'MyResidentEvil.MedicalItemController');



-- from FrontPolicementGate to FrontHall            (9.05, 0, 3.73)                0

-- from FrontHall to FirstFloorCorridor             (1.627, 0, 1.174)              0
-- from FrontHall to PoliceOffice                   (1.19, 0, 2.701)               90

-- from PoliceOffice to FrontHall                   (1.37, 0, 22.2)                90

-- from CasinoRoom to FirstFloorCorridor            (10.446, 0, 12.91)             0

-- from FirstFloorCorridor to FrontHall             (1.371, 0, 5.69)               90
-- from FirstFloorCorridor to CasinoRoom            (-2.909, 0.56, -6.725)         0
-- from FirstFloorCorridor to PowerSourceRoom       (3.009, 0, 8.21)               90
-- from FirstFloorCorridor to SecondFloorCorridor   (1.62, 0, -1.874)              0

-- from PowerSourceRoom to FirstFloorCorridor       (19.1, 0, 2.62)                0

-- from SecondFloorCorridor to FirstFloorCorridor   (23.492, 3.016, 1.039)         0
-- from SecondFloorCorridor to BattleRoom           (6.527, 0, 1.177)              0
-- from SecondFloorCorridor to HelicopterParkingStation (-3.71, 0, 0.78)           120

-- from BattleRoom to SecondFloorCorridor           (0.976, 0, 8.12)               90


