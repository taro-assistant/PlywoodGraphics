#!/usr/bin/env python3
"""
ProtonMail アカウント作成 - Mac Pro 2013対応版
OpenGLフラグ付きでChrome起動
"""

from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.chrome.service import Service
import time
import random
import string
import json
import subprocess

# アカウント情報生成
USERNAME_PREFIX = "taro_assistant"
suffix = ''.join(random.choices(string.ascii_lowercase + string.digits, k=4))
USERNAME = f"{USERNAME_PREFIX}_{suffix}"

def generate_password(length=16):
    chars = string.ascii_letters + string.digits + "!@#$%^&*"
    return ''.join(random.choice(chars) for _ in range(length))

PASSWORD = generate_password()
EMAIL = f"{USERNAME}@proton.me"

def main():
    print("🦞 ProtonMail アカウント作成 (Mac Pro 2013対応)")
    print("=" * 60)
    
    # Chrome設定 - OpenGLフラグ追加
    options = Options()
    options.binary_location = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome"
    
    # Mac Pro 2013 / OCLP対策: OpenGLで描画
    options.add_argument("--use-angle=gl")
    options.add_argument("--disable-metal")
    
    options.add_argument("--no-sandbox")
    options.add_argument("--disable-dev-shm-usage")
    options.add_argument("--disable-blink-features=AutomationControlled")
    options.add_experimental_option("excludeSwitches", ["enable-automation"])
    
    service = Service("/usr/local/bin/chromedriver-146")
    driver = webdriver.Chrome(service=service, options=options)
    
    try:
        print("🌐 ProtonMailを開く: https://proton.me/signup")
        print("⚙️ OpenGL描画モードで起動（Mac Pro 2013対策）")
        driver.get("https://proton.me/signup")
        time.sleep(4)
        
        print("\n📋 手順:")
        print("1. 'Create account' または 'Sign up for free' をクリック")
        print("2. 'Continue with free' を選択")
        print(f"3. ユーザー名入力: {USERNAME}")
        print("4. ドメイン: proton.me を選択")
        print(f"5. パスワード: {PASSWORD}")
        print("6. 確認パスワードを入力")
        print("7. 復旧メールはSkip")
        print("8. Create account")
        print("\n🚨 人間確認（CAPTCHA/SMS）が出たら手動で対応")
        print("=" * 60)
        
        # アカウント情報を保存
        account_info = {
            "email": EMAIL,
            "username": USERNAME,
            "password": PASSWORD,
            "created_at": time.strftime("%Y-%m-%d %H:%M:%S"),
            "purpose": "Moltbook registration",
            "platform": "ProtonMail"
        }
        
        # ファイルに保存
        filepath = "/Users/admin/.openclaw/workspace/protonmail_credentials.json"
        with open(filepath, "w") as f:
            json.dump(account_info, f, indent=2)
        
        # Notes.app用テキスト
        notes_text = f"""
【ProtonMail アカウント】
メール: {EMAIL}
ユーザー名: {USERNAME}
パスワード: {PASSWORD}
作成: {time.strftime("%Y-%m-%d %H:%M:%S")}
用途: Moltbook登録用
"""
        
        notes_path = "/Users/admin/.openclaw/workspace/protonmail_credentials.txt"
        with open(notes_path, "w") as f:
            f.write(notes_text)
        
        print(f"\n💾 保存済み:")
        print(f"   JSON: {filepath}")
        print(f"   TXT:  {notes_path}")
        print(f"\n📧 メール: {EMAIL}")
        print(f"🔑 パスワード: {PASSWORD}")
        print("\n✅ Notes.appにコピーしてください")
        print("⏳ ブラウザを開いたままにします...")
        print("   （人間確認完了後、Enterキーを押してください）")
        
        input("\n[Enterキーで終了] ...")
        
        return EMAIL, PASSWORD
        
    finally:
        driver.quit()

if __name__ == "__main__":
    main()
