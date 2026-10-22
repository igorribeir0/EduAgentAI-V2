
import { useEffect, useRef, useState } from 'react'
import type { FormEvent } from 'react'
import AuthPage from './components/AuthPage'
import type { AuthSession } from './components/AuthPage'
import './App.css'

type ChatMessage = {
  id: string
  role: 'student' | 'assistant'
  content: string
}

type ConversationSummary = {
  id: string
  title: string
  createdAt: string
}

type SavedMessage = {
  role: number
  content: string
  createdAt: string
}

type ChatResponse = {
  conversationId: string
  answer: string
}

function App() {
  // JWT mantido apenas em memória
  const [session, setSession] = useState<AuthSession | null>(null)

  const [conversationId, setConversationId] = useState<string | null>(null)
  const [conversations, setConversations] =
    useState<ConversationSummary[]>([])
  const [messages, setMessages] = useState<ChatMessage[]>([])
  const [message, setMessage] = useState('')
  const [loading, setLoading] = useState(false)
  const [historyLoading, setHistoryLoading] = useState(false)
  const [error, setError] = useState('')

  const bottomRef = useRef<HTMLDivElement>(null)
  const requestRef = useRef(0)
  const busyRef = useRef(false)

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [messages, loading])

  function handleLogout() {
    if (busyRef.current) return

    requestRef.current++
    setSession(null)
    setConversationId(null)
    setConversations([])
    setMessages([])
    setMessage('')
    setError('')
    setHistoryLoading(false)
  }

  function handleAuthenticated(auth: AuthSession) {
    requestRef.current++
    setSession(auth)
    setConversationId(null)
    setConversations([])
    setMessages([])
    setMessage('')
    setError('')
  }

  // Carrega o histórico sempre que o aluno faz login
  useEffect(() => {
    if (!session) return

    const controller = new AbortController()

    async function loadHistory() {
      try {
        const response = await fetch('/api/conversations', {
          headers: {
            Authorization: `Bearer ${session!.token}`,
          },
          signal: controller.signal,
        })

        if (response.status === 401) {
          setSession(null)
          setError('')
          return
        }

        if (!response.ok) {
          throw new Error(`HTTP ${response.status}`)
        }

        const data: ConversationSummary[] = await response.json()

        if (!controller.signal.aborted) {
          setConversations(data)
        }
      } catch (err) {
        if (!controller.signal.aborted) {
          console.error(err)
          setError('Não foi possível carregar o histórico.')
        }
      }
    }

    void loadHistory()

    return () => controller.abort()
  }, [session])

  async function refreshConversations(token: string) {
    const response = await fetch('/api/conversations', {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })

    if (response.status === 401) {
      setSession(null)
      return
    }

    if (!response.ok) {
      throw new Error(`Erro ao carregar histórico: ${response.status}`)
    }

    const data: ConversationSummary[] = await response.json()
    setConversations(data)
  }

  async function openConversation(id: string) {
    if (!session || busyRef.current || historyLoading) return

    const request = ++requestRef.current

    setHistoryLoading(true)
    setError('')

    try {
      const response = await fetch(
        `/api/conversations/${encodeURIComponent(id)}/messages`,
        {
          headers: {
            Authorization: `Bearer ${session.token}`,
          },
        }
      )

      if (response.status === 401) {
        setSession(null)
        return
      }

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}`)
      }

      const data: SavedMessage[] = await response.json()

      if (request !== requestRef.current) return

      setConversationId(id)

      setMessages(
        data
          .filter(item => item.role === 1 || item.role === 2)
          .map(item => ({
            id: crypto.randomUUID(),
            role: item.role === 1 ? 'student' : 'assistant',
            content: item.content,
          }))
      )
    } catch (err) {
      if (request === requestRef.current) {
        console.error(err)
        setError('Não foi possível abrir essa conversa.')
      }
    } finally {
      if (request === requestRef.current) {
        setHistoryLoading(false)
      }
    }
  }

  async function sendMessage(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!session || busyRef.current || historyLoading) return

    const text = message.trim()

    if (!text || text.length > 4000) return

    busyRef.current = true
    setLoading(true)
    setError('')

    try {
      const response = await fetch('/api/chat/message', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${session.token}`,
        },
        body: JSON.stringify({
          conversationId,
          message: text,
        }),
      })

      if (response.status === 401) {
        setSession(null)
        return
      }

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}`)
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

      try {
        await refreshConversations(session.token)
      } catch (historyError) {
        console.error('Erro ao atualizar histórico:', historyError)
        setError(
          'Mensagem enviada, mas não foi possível atualizar o histórico.'
        )
      }
    } catch (err) {
      console.error(err)
      setError('Não foi possível enviar sua mensagem.')
    } finally {
      busyRef.current = false
      setLoading(false)
    }
  }

  function newConversation() {
    if (busyRef.current) return

    requestRef.current++
    setHistoryLoading(false)
    setConversationId(null)
    setMessages([])
    setMessage('')
    setError('')
  }

  // Sem JWT, o usuário deve fazer login
  if (!session) {
    return <AuthPage onAuthenticated={handleAuthenticated} />
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

        <div className="conversation-section">
          <h3>Histórico</h3>

          {conversations.length === 0 ? (
            <p className="empty-history">
              Nenhuma conversa ainda.
            </p>
          ) : (
            <div className="conversation-list">
              {conversations.map(item => (
                <button
                  key={item.id}
                  className={`conversation-item ${
                    conversationId === item.id ? 'active' : ''
                  }`}
                  onClick={() => openConversation(item.id)}
                  disabled={loading || historyLoading}
                  title={item.title}
                >
                  {item.title}
                </button>
              ))}
            </div>
          )}
        </div>

        <div className="sidebar-user">
          <div className="user-avatar">
            {session.fullName.charAt(0).toUpperCase()}
          </div>

          <div className="user-details">
            <strong>{session.fullName}</strong>
            <span>{session.email}</span>
          </div>
        </div>

        <button
          className="logout-button"
          onClick={handleLogout}
          disabled={loading}
        >
          Sair da conta
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
            <p>
              Olá, {session.fullName}! Vamos continuar aprendendo?
            </p>
          </div>

          <span className="ai-tag">EduAgent AI</span>
        </header>

        <section className="chat-area">
          {historyLoading ? (
            <div className="welcome">
              <p>Carregando conversa...</p>
            </div>
          ) : messages.length === 0 ? (
            <div className="welcome">
              <div className="welcome-icon">✦</div>

              <h2>Olá! Como posso ajudar?</h2>

              <p>
                Tire dúvidas sobre idiomas, peça dicas de estudos
                e converse com nosso assistente de inteligência
                artificial.
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
                    {item.role === 'student'
                      ? 'Você'
                      : 'EduAgent AI'}
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
              disabled={loading || historyLoading}
            />

            <button
              type="submit"
              disabled={
                loading ||
                historyLoading ||
                !message.trim()
              }
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
