import os
from datetime import datetime, timedelta
from typing import Optional
from jose import JWTError, jwt
from passlib.context import CryptContext
from fastapi import Depends, HTTPException, status
from fastapi.security import OAuth2PasswordBearer, OAuth2PasswordRequestForm

# --- 비밀번호 해싱 설정 ---
pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")

# --- JWT 설정 ---
# 보안을 위해 실제 환경에서는 환경 변수나 별도의 설정 파일을 사용하는 것이 좋습니다.
SECRET_KEY = "your-secret-key"  # 실제 배포 시에는 반드시 강력한 키로 변경하세요.
ALGORITHM = "HS256"
ACCESS_TOKEN_EXPIRE_MINUTES = 15

# 기존 해시된 비밀번호 (사용자: admin)
HASHED_PASSWORD = "$2b$12$IvgDqkXeVL4yRbET0W94vu8LG3tnhmWPjcrvsmMQ0Y3tnVk29TwQu"

# OAuth2 스킴 정의
oauth2_scheme = OAuth2PasswordBearer(tokenUrl="/api/login")

# --- 함수 정의 ---

def verify_password(plain_password, hashed_password):
    """입력된 비밀번호와 해시된 비밀번호를 비교합니다."""
    return pwd_context.verify(plain_password, hashed_password)

def create_access_token(data: dict, expires_delta: Optional[timedelta] = None):
    """주어진 데이터로 JWT 액세스 토큰을 생성합니다."""
    to_encode = data.copy()
    if expires_delta:
        expire = datetime.utcnow() + expires_delta
    else:
        expire = datetime.utcnow() + timedelta(minutes=15)
    to_encode.update({"exp": expire})
    encoded_jwt = jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)
    return encoded_jwt

async def get_current_user(token: str = Depends(oauth2_scheme)):
    """토큰을 검증하고 현재 사용자를 반환합니다."""
    credentials_exception = HTTPException(
        status_code=status.HTTP_401_UNAUTHORIZED,
        detail="Could not validate credentials",
        headers={"WWW-Authenticate": "Bearer"},
    )
    try:
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
        username: str = payload.get("sub")
        if username is None:
            raise credentials_exception
        # 여기서는 간단히 사용자 이름만 반환하지만,
        # 실제 앱에서는 데이터베이스에서 사용자 정보를 조회할 수 있습니다.
        user = {"username": username}
    except JWTError:
        raise credentials_exception
    return user
