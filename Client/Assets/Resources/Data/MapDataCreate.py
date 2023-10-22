import pandas as pd
import json

# 엑셀 파일과 원하는 시트의 이름을 지정합니다.
excel_file = 'MapDataExel.xlsm'
sheet_name = 'DataCreate'  # 원하는 시트의 이름으로 변경하세요.
# 엑셀 파일을 pandas DataFrame으로 읽어옵니다.
df = pd.read_excel(excel_file, sheet_name=sheet_name)

# 엑셀 파일을 pandas DataFrame으로 읽어옵니다.
df = pd.read_excel(excel_file, sheet_name=sheet_name)

# DataFrame을 JSON 형식으로 변환합니다.
data = []
for index, row in df.iterrows():
    record = {
        "mapId": str(row['mapId']),
        "name": row['name'],
        "townName": row['townName'],
        "townId": row['townId'],
        "soundPath": row['soundPath'],
        "spawnTime": row['spawnTime'],
        "monsters": []
    }
    
    # # monsters 항목을 처리합니다.
    # for i in range(1, 3):  # monsters 열의 개수에 따라 범위를 조절하세요.
    #     monster_str = row[f'monsters{i}']
    #     if pd.notna(monster_str):
    #         monsterId, count = map(str, monster_str.split('/'))
    #         record["monsters"].append({
    #             "monsterId": monsterId,
    #             "count": count
    #         })


    # monsters 항목을 처리합니다.
    for i in range(1, 4):  # 열(column) 번호에 따라 범위를 조절하세요.
        monster_id = row[f'mID{i}']
        monster_count = row[f'mQ{i}']
        
        if pd.notna(monster_id) and pd.notna(monster_count):
            record["monsters"].append({
                "monsterId": str(int(monster_id)),
                "count": str(int(monster_count))
            })

    
    data.append(record)

# "maps" 객체로 감싸서 저장합니다.
maps_data = {"maps": data}

# JSON 파일로 저장합니다.
with open('MapData.json', 'w', encoding='utf-8') as json_file:
    json.dump(maps_data, json_file, ensure_ascii=False, indent=4)