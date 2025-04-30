# 🪶 SoulWriter

**Your voice. Your soul. Amplified by AI.**

SoulWriter is a collaborative AI writing agent built with [Microsoft Semantic Kernel](https://github.com/microsoft/semantic-kernel), designed to help mindful creators turn raw thoughts into polished, platform-ready content. It goes beyond generation — it thinks *with* you.

---

## ✨ What It Does

SoulWriter lets you:
- 🧠 **Brainstorm** interactively from your personal notes
- 📌 **Save and refine talking points** that resonate with you
- ✍️ **Generate full-length blog articles** in your voice
- 📰 **Format posts for Medium** with best practices
- 💬 Do all of this through a clean, conversational chat UI

You control the process. The agents do the heavy lifting.

---

## 🧱 Tech Stack

- ✅ **Blazor Web App** for full-stack C# experience
- ✅ **TailwindCSS** for clean UI styling
- ✅ **Microsoft Semantic Kernel** with multi-agent orchestration
- ✅ **CosmosDB** (or in-memory) for talking point storage
- ✅ **OpenAI / Azure OpenAI** as the LLM backend

---

## 🧠 Architecture Overview

SoulWriter uses an **agent router** to direct user input to the right skill:

| Agent | Role |
|-------|------|
| `BrainstormingAgent` | Explores ideas collaboratively from user notes |
| `TalkingPointsStore` | Adds, lists, deletes saved insights |
| `DraftWritingAgent` | Generates articles from curated talking points |
| `PlatformFormatterAgent` | Formats final drafts for Medium |
| `RouterAgent` | Analyzes input and routes to the right agent + intent |

All logic and memory are modeled explicitly — no “magic” black box.

---

## 🧪 Try These Sample Prompts

```txt
I want to explore how rest only works when it's guilt-free
→ 🧠 BrainstormingAgent

Add that to my list
→ 📌 TalkingPointsStore (Add)

What have I saved?
→ 📌 TalkingPointsStore (List)

Write a blog post from my saved points
→ ✍️ DraftWritingAgent

Format that for Medium
→ 📰 PlatformFormatterAgent
```

---

## 🎥 Demo Video

> Coming soon: A short walkthrough showing a full user journey from idea to publication.

---

## 💡 Why SoulWriter?

Most AI writing tools force you to fit into their structure.  
SoulWriter adapts to **your thoughts**, **your process**, and **your voice**.

It is not just an assistant.  
It is your thinking partner.

---

## 📦 Setup Instructions

1. Clone this repo
2. Run the Blazor Web App
3. Add your OpenAI/Azure OpenAI keys
4. (Optional) Connect CosmosDB or use in-memory talking point storage
5. Start writing with Soul.

---

## 🧩 Agents Powered By

- Semantic Kernel’s `ChatCompletionAgent` and YAML-based planning
- Memory-aware context using CosmosDB or custom services
- Natural language intent routing via `RouterAgent`

---

## 📜 License

MIT — use it, remix it, or build your own soul-powered writing tool.

