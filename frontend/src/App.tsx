
import { useEffect, useRef, useState } from 'react'
import type { FormEvent } from 'react'
import './App.css'

type ChatMessage = {
  id: string
  role: 'student' | 'assistant'
  content: string
}

type ChatResponse = {
  conversationId: string
  answer: string
}

const STUDENT_KEY = 'eduagent-student-id'

function getStudentId(): string {
  let id = sessionStorage.getItem(STUDENT_KEY)

  if (!id) {
    id = crypto.randomUUID()
    sessionStorage.setItem(STUDENT_KEY, id)
  }

  return id
}

function App() {
  const [studentId] = useState(getStudentId)
  const [conversationId, setConversationId] = useState<string | null>(null)
  const [messages, setMessages] = useState<ChatMessage[]>([])
  const [message, setMessage] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const bottomRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [messages, loading])

  async function sendMessage(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    const text = message.trim()
    if (!text || loading) return

    setLoading(true)
    setError('')

    try {
      const response = await fetch('/api/chat/message', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          studentId,
          conversationId,
          message: text,
        }),
      })

      if (!response.ok) {
        throw new Error(`Erro ao enviar mensagem: HTTP ${response.status}`)
      }

      const data: ChatResponse = await response.json()

      setMessages(previous => [
        ...previous,
        {
          id: crypto.randomUUID(),
          role: 'student',
          content: text,
        },
        {
          id: crypto.randomUUID(),
          role: 'assistant',
          content: data.answer,
        },
      ])

      setConversationId(data.conversationId)
      setMessage('')
    } catch (err) {
      setError(
        err instanceof Error ? err.message : 'Erro ao conectar com a API.'
      )
    } finally {
      setLoading(false)
    }
  }

  function newConversation() {
    if (loading) return

    setConversationId(null)
    setMessages([])
    setMessage('')
    setError('')
  }

  return (
    <div className="app">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-icon">E</div>
          <div>
            <strong>EduAgentAI</strong>
            <span>Assistente Educacional</span>
          </div>
        </div>

        <button
          className="new-chat"
          onClick={newConversation}
          disabled={loading}
        >
          + Nova conversa
        </button>

        <div className="sidebar-bottom">
          <span className="status-dot" />
          Gemini AI integrado
        </div>
      </aside>

      <main className="main">
        <header className="header">
          <div>
            <h1>Assistente de Idiomas</h1>
            <p>Seu assistente virtual para aprender idiomas</p>
          </div>
          <span className="ai-tag">EduAgent AI</span>
        </header>

        <section className="chat-area">
          {messages.length === 0 ? (
            <div className="welcome">
              <div className="welcome-icon">✦</div>
              <h2>Olá! Como posso ajudar?</h2>
              <p>
                Tire dúvidas sobre idiomas, peça dicas de estudos
                e converse com nosso assistente de inteligência artificial.
              </p>
            </div>
          ) : (
            <div className="messages">
              {messages.map(item => (
                <div
                  key={item.id}
                  className={`message ${
                    item.role === 'student' ? 'student' : 'assistant'
                  }`}
                >
                  <div className="message-label">
                    {item.role === 'student' ? 'Você' : 'EduAgent AI'}
                  </div>

                  <div className="message-content">
                    {item.content}
                  </div>
                </div>
              ))}

              {loading && (
                <div className="typing">
                  EduAgent AI está digitando...
                </div>
              )}

              <div ref={bottomRef} />
            </div>
          )}
        </section>

        <div className="composer">
          {error && (
            <div className="error" role="alert">
              {error}
            </div>
          )}

          <form onSubmit={sendMessage}>
            <input
              value={message}
              onChange={event => setMessage(event.target.value)}
              placeholder="Digite sua mensagem..."
              maxLength={4000}
              disabled={loading}
            />

            <button
              type="submit"
              disabled={loading || !message.trim()}
            >
              {loading ? 'Enviando...' : 'Enviar ➤'}
            </button>
          </form>

          <small>
            EduAgentAI utiliza inteligência artificial.
            As respostas podem conter imprecisões.
          </small>
        </div>
      </main>
    </div>
  )
}

export default App
