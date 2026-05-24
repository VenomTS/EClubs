import argparse
import json

import pyexcel as pe
import requests


def extractData(filePath):
    try:
        sheet = pe.get_sheet(file_name=filePath)
        data = sheet.to_array()
        return data

    except Exception as e:
        print(f"Error reading file: {e}")
        return []

def beautifyData(array):
    beautifulData = []
    for data in array:
        if '' in data or "Nastavna jedinica" in data:
            continue

        beautifulData.append([data[1].replace('.', ''), data[3].replace('.', ''), data[4].replace('.', ''), data[5].replace('.', '')])
    return beautifulData

def convertToJson(rows):
    headers = ["Domain", "Unit", "LearningOutcome", "Indicator"]
    n = len(headers)

    result = []
    for row in rows:
        item = {}

        for i in range(n):
            item[headers[i]] = row[i]

        result.append(item)
    return json.dumps(result, indent=4, ensure_ascii=False)

def sendPostRequest(clubId, jsonData):

    requests.post(
        f"https://192.168.1.101:8080/api/clubs/{clubId}/WorkPlans/batch",
        headers={
            "Content-Type": "application/json"
        },
        json=json.loads(jsonData),
        verify=False
    )
    
def main():

    parser = argparse.ArgumentParser()

    parser.add_argument("-i", "--input", required=True, help="Input File (.xlsx)")
    parser.add_argument("-cId", "--clubId", required=True, help="Club ID")

    args = parser.parse_args()

    fileName = args.input
    clubId = args.clubId

    rows = extractData(fileName)
    beautifulData = beautifyData(rows)

    jsonResult = convertToJson(beautifulData)

    sendPostRequest(clubId, jsonResult)



if __name__ == "__main__":
    main()