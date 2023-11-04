import pandas as pd
import json

# 엑셀 파일 로드
excel_file = 'QuestDataExel.xlsm'

# 각 탭에 대한 데이터프레임 생성
quests_df = pd.read_excel(excel_file, sheet_name='quests')
dialogue_df = pd.read_excel(excel_file, sheet_name='dialogue')
reward_df = pd.read_excel(excel_file, sheet_name='reward')

# JSON 데이터 구조 생성
quests_data = {"quests": []}

# quests 탭 처리
for _, quest_row in quests_df.iterrows():
    quest_dict = {
        "questId": str(quest_row['questId']),
        "questName": quest_row['questName'],
        "reqQuest": str(quest_row['reqQuest']),
        "reqJob": str(quest_row['reqJob']),
        "reqLev": str(quest_row['reqLev']),
        "npc": str(quest_row['npc']),
        "status": str(quest_row['status']),
        "desc": quest_row['desc'],
        "dialogue": []
    }
    
    # 해당 questId에 대한 대화(dialogue) 데이터 처리
    quest_dialogues = dialogue_df[dialogue_df['questId'] == quest_row['questId']]
    for _, dialogue_row in quest_dialogues.iterrows():
        dialogue_dict = {
            "index": str(dialogue_row['index']),
            "script": dialogue_row['script']
        }
        if pd.notna(dialogue_row['statusChange']):
            dialogue_dict["statusChange"] = str(dialogue_row['statusChange'])
        if 'getItem' in dialogue_row and pd.notna(dialogue_row['getItem']):
            dialogue_dict["getItem"] = json.loads(dialogue_row['getItem'].replace("'", '"'))
        if 'loseItem' in dialogue_row and pd.notna(dialogue_row['loseItem']):
            dialogue_dict["loseItem"] = json.loads(dialogue_row['loseItem'].replace("'", '"'))
        quest_dict["dialogue"].append(dialogue_dict)

    # quest 데이터 추가
    quests_data['quests'].append(quest_dict)

# JSON 파일로 저장
with open('QuestData.json', 'w', encoding='utf-8') as json_file:
    json.dump(quests_data, json_file, ensure_ascii=False, indent=2)