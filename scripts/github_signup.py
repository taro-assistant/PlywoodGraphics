#!/usr/bin/env python3
"""
GitHub アカウント作成 - Mac Pro 2013対応版
"""

from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.chrome.service import Service
import time
import random
import string
import json

# アカウント情報（ProtonMail使用）
EMAIL = "taro_assistant_7ryi@proton.me"  # 先ほど作成したProtonMail
USERNAME_PREFIX = "taro-assistant"
suffix = ''.join(random.choices(string.ascii_lowercase + string.digits, k=4))
USERNAME = f"{USERNAME_PREFIX}-{suffix}"

def generate_password(length=20):
    chars = string.ascii_letters + string.digits + "!@#$%^&*"
    return ''.join(random.choice(chars) for _ in range(length))

PASSWORD = generate_password()

def main():
    print("🐙 GitHub アカウント作成")
    print("=" * 60)
    
    # Chrome設定 - OpenGLフラグ追加（Mac Pro 2013対策）
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
        print("🌐 GitHubサインアップページを開く")
        print("⚙️ OpenGL描画モードで起動（Mac Pro 2013対策）")
        driver.get("https://github.com/signup")
        time.sleep(4)
        
        print("\n📋 手順:")
        print(f"1. メールアドレス入力: {EMAIL}")
        print("2. 'Continue' をクリック")
        print(f"3. パスワード作成: {PASSWORD}")
        print("4. ユーザー名入力（自動生成または手動）")
        print(f"   推奨: {USERNAME}")
        print("5. 'y'または'n'でメール配信設定")
        print("6. CAPTCHA認証（人間確認）")
        print("7. 確認メールをProtonMailで受信")
        print("=" * 60)
        
        # アカウント情報を保存
        account_info = {
            "email": EMAIL,
            "username": USERNAME,
            "password": PASSWORD,
            "created_at": time.strftime("%Y-%m-%d %H:%M:%S"),
            "platform": "GitHub",
            "email_provider": "ProtonMail"
        }
        
        # ファイルに保存
        filepath = "/Users/admin/.openclaw/workspace/github_credentials.json"
        with open(filepath, "w") as f:
            json.dump(account_info, f, indent=2)
        
        # Notes.app用テキスト
        notes_text = f"""
【GitHub アカウント】
メール: {EMAIL}
ユーザー名: {USERNAME}
パスワード: {PASSWORD}
作成: {time.strftime("%Y-%m-%d %H:%M:%S")}
メールプロバイダ: ProtonMail
"""
        
        notes_path = "/Users/admin/.openclaw/workspace/github_credentials.txt"
        with open(notes_path, "w") as f:
            f.write(notes_text)
        
        print(f"\n💾 保存済み:")
        print(f"   JSON: {filepath}")
        print(f"   TXT:  {notes_path}")
        print(f"\n📧 メール: {EMAIL}")
        print(f"👤 ユーザー名: {USERNAME}")
        print(f"🔑 パスワード: {PASSWORD}")
        print("\n✅ Notes.appにコピーしてください")
        print("⏳ ブラウザを開いたままにします...")
        print("   （確認メール受信後、Enterキーを押してください）")
        
        input("\n[Enterキーで終了] ...")
        
        return EMAIL, USERNAME, PASSWORD
        
    finally:
        driver.quit()

if __name__ == "__main__":
    main()
