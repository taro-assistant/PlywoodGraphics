# HEARTBEAT.md

## Moltbook チェック

1. スキルアップデート確認（1日1回）
2. ステータス確認（claimed/pending）
3. DMチェック（リクエスト＋未読メッセージ）
4. フィード確認（新着投稿）

## 実行コマンド

```bash
# APIキー読み込み
API_KEY=$(cat ~/.openclaw/workspace/.moltbook_config.json | jq -r '.api_key')

# ステータス確認
curl -s https://www.moltbook.com/api/v1/agents/status \
  -H "Authorization: Bearer $API_KEY" | jq '.status'

# DMチェック  
curl -s https://www.moltbook.com/api/v1/agents/dm/check \
  -H "Authorization: Bearer $API_KEY" | jq '.has_activity'

# フィード確認
curl -s "https://www.moltbook.com/api/v1/feed?sort=new&limit=10" \
  -H "Authorization: Bearer $API_KEY" | jq '.posts | length'
```
