import pandas as pd
import json

# 엑셀 파일과 원하는 시트의 이름을 지정합니다.
excel_file = '몬스터.xlsm'
sheet_name = 'Sheet3'  # 원하는 시트의 이름으로 변경하세요.
# 엑셀 파일을 pandas DataFrame으로 읽어옵니다.
df = pd.read_excel(excel_file, sheet_name=sheet_name)

# 엑셀 파일을 pandas DataFrame으로 읽어옵니다.
df = pd.read_excel(excel_file, sheet_name=sheet_name)

# DataFrame을 JSON 형식으로 변환합니다.
data = []
for index, row in df.iterrows():
    record = {
        "id": str(row['id']),
        "name": row['name'],
        "stat": {
            "level": str(row['stat-level']),
            "maxHp": str(row['stat-maxHp']),
            "attack": str(row['stat-attack']),
            "speed": str(row['stat-speed']),
            "exp": str(row['stat-exp']),
            "dex": str(row['stat-dex'])
        },
        "HitSoundPath": row['HitSoundPath'],
        "rewards": []
    }
    
    # rewards 항목을 처리합니다.
    for i in range(1, 4):  # rewards 열의 개수에 따라 범위를 조절하세요.
        reward_str = row[f'rewards{i}']
        if pd.notna(reward_str):
            probability, itemId, count = map(str, reward_str.split('/'))
            record["rewards"].append({
                "probability": probability,
                "itemId": itemId,
                "count": count
            })
    
    data.append(record)

# "monsters" 객체로 감싸서 저장합니다.
monsters_data = {"monsters": data}

# JSON 파일로 저장합니다.
with open('MonsterData.json', 'w', encoding='utf-8') as json_file:
    json.dump(monsters_data, json_file, ensure_ascii=False, indent=4)