import pandas as pd
import json

# Excel 파일 경로
excel_file_path = 'QuestDataExel.xlsm'

# 각 탭을 읽어 DataFrame으로 변환
quests_df = pd.read_excel(excel_file_path, sheet_name='quests')
dialogue_df = pd.read_excel(excel_file_path, sheet_name='dialogue')
reward_df = pd.read_excel(excel_file_path, sheet_name='reward')

# quests 탭을 딕셔너리 리스트로 변환
quests_data = quests_df.to_dict('records')

# 최종 quests JSON 객체 생성
quests_json = []

for quest in quests_data:
    # 각 퀘스트 별로 dialogue와 reward를 매칭
    quest_dialogues = dialogue_df[dialogue_df['questId'] == quest['questId']].to_dict('records')
    quest_rewards = reward_df[reward_df['questId'] == quest['questId']].to_dict('records')
    
    # Dialogue 리스트를 구성
    dialogue_list = []
    for dialogue in quest_dialogues:
        dialogue_entry = {
            "index": str(dialogue['index']),
            "script": dialogue['script']
        }
        # statusChange가 null이 아니라면 추가
        if pd.notnull(dialogue['statusChange']) and dialogue['statusChange'] != 'null':
            dialogue_entry['statusChange'] = str(int(dialogue['statusChange']))  # 정수로 변환
        # getItem과 loseItem을 추가
        for reward in quest_rewards:
            if reward['index'] == dialogue['index']:
                if reward['kind'] == 'getItem':
                    if 'getItem' not in dialogue_entry:
                        dialogue_entry['getItem'] = []
                    dialogue_entry['getItem'].append({
                        "itemId": str(reward['itemId']),
                        "quantity": str(reward['quantity'])
                    })
                elif reward['kind'] == 'loseItem':
                    if 'loseItem' not in dialogue_entry:
                        dialogue_entry['loseItem'] = []
                    dialogue_entry['loseItem'].append({
                        "itemId": str(reward['itemId']),
                        "quantity": str(reward['quantity'])
                    })
                elif reward['kind'] == 'getJob':
                    if 'getJob' not in dialogue_entry:
                        dialogue_entry['getJob'] = []
                    dialogue_entry['getJob'].append({
                        "itemId": str(reward['itemId']),
                        "quantity": str(reward['quantity'])
                    })

        dialogue_list.append(dialogue_entry)
    
    # 퀘스트 정보 추가
    quests_json.append({
        "questId": str(quest['questId']),
        "questName": quest['questName'],
        "reqQuest": str(quest['reqQuest']),
        "reqJob": str(quest['reqJob']),
        "reqLev": str(quest['reqLev']),
        "npc": str(quest['npc']),
        "status": str(quest['status']),
        "desc": quest['desc'],
        "dialogue": dialogue_list
    })

# JSON 파일로 저장
with open('QuestData.json', 'w', encoding='utf-8') as json_file:
    json.dump({"quests": quests_json}, json_file, ensure_ascii=False, indent=2)
