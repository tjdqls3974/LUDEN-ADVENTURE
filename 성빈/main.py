from fastapi import FastAPI, Depends, HTTPException, status, Request, File, UploadFile, Form
from fastapi.security import OAuth2PasswordRequestForm
from fastapi.responses import JSONResponse, FileResponse
from fastapi.staticfiles import StaticFiles
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
import uvicorn
import os
from typing import List
import shutil
import json
import csv

# --- 인증 관련 임포트 (기존 유지) ---
# from auth import create_access_token, get_current_user, verify_password, HASHED_PASSWORD 
# (auth 파일이 없어서 주석 처리했습니다. 실제 사용 시에는 주석 해제하세요)

app = FastAPI()

# --- CORS 설정 ---
origins = [
    "http://localhost:5173",
    "http://127.0.0.1:5173",
    "http://2-6.site",
    "*" 
]

app.add_middleware(
    CORSMiddleware,
    allow_origins=origins,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# --- 데이터 모델 ---
class Token(BaseModel):
    access_token: str
    token_type: str

class User(BaseModel):
    username: str

class idch(BaseModel):
    stnum: int

class CsvContent(BaseModel):
    name: str
    stnum: int
    contacts: str
    cleared: bool 
    attempts: int
    record: str
    distance: str

# --- 경로 설정 ---
BASE_DIR = os.path.join("./data/")
CSV_PATH = os.path.join(BASE_DIR, "data.csv")

if not os.path.exists(BASE_DIR):
    os.makedirs(BASE_DIR)

def read_csv():
    if not os.path.exists(CSV_PATH):
        return []
    with open(CSV_PATH, "r", encoding="utf-8") as f:
        reader = csv.DictReader(f)
        return list(reader)

def check_csv_exists(stnum: int):
    csv_data = read_csv()
    for i in csv_data:
        if i.get('stnum') == str(stnum):
            return True
    return False

def add_csv(name: str, stnum: int, contacts: str, cleared: bool, attempts: int, record: str, distance: str):
    file_exists = os.path.isfile(CSV_PATH)
    
    with open(CSV_PATH, "a", encoding="utf-8", newline='') as f:
        fieldnames = ["name", "stnum", "contacts", "cleared", "attempts", "record","distance"]
        writer = csv.DictWriter(f, fieldnames=fieldnames)
        
        if not file_exists:
            writer.writeheader()

        if not check_csv_exists(stnum):
            writer.writerow({
                "name": name, 
                "stnum": stnum, 
                "contacts": contacts, 
                "cleared": cleared, 
                "attempts": attempts, 
                "record": record,
                "distance": distance
            })

# [새로 추가된 함수] CSV 행 삭제 로직
def delete_csv_row(stnum: int):
    if not os.path.exists(CSV_PATH):
        return False

    rows = read_csv()
    original_count = len(rows)
    
    # stnum이 다른 것만 남김 (필터링)
    new_rows = [row for row in rows if row.get('stnum') != str(stnum)]

    if len(new_rows) == original_count:
        return False # 삭제된 것이 없음

    # 덮어쓰기
    with open(CSV_PATH, "w", encoding="utf-8", newline='') as f:
        fieldnames = ["name", "stnum", "contacts", "cleared", "attempts", "record", "distance"]
        writer = csv.DictWriter(f, fieldnames=fieldnames)
        writer.writeheader()
        writer.writerows(new_rows)
    return True

# --- API 엔드포인트 ---
@app.get("/api/csv")
async def get_data_json():
    return read_csv()

@app.post("/api/csv")
async def update_csv(csv_content: CsvContent):
    add_csv(csv_content.name, csv_content.stnum, csv_content.contacts, csv_content.cleared, csv_content.attempts, csv_content.record, csv_content.distance)
    return {"message": "csv updated successfully"}

# [새로 추가된 엔드포인트] 데이터 삭제
@app.delete("/api/csv")
async def remove_csv_data(data: idch):
    result = delete_csv_row(data.stnum)
    if result:
        return {"message": f"Student {data.stnum} deleted successfully"}
    else:
        raise HTTPException(status_code=404, detail="Student number not found")

@app.post("/api/check")
async def check_csv(data: idch):
    return {"isPlayed": check_csv_exists(data.stnum)}

# --- 서버 실행 ---
if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=50208)