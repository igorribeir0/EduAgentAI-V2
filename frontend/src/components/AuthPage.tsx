import { useState } from 'react'
import type { FormEvent } from 'react'
import './AuthPage.css'

export type AuthSession = {
  token: string
  expiresAt: string
  userId: string
  fullName: string
  email: string
}

type AuthPageProps = {
  onAuthenticated: (session: AuthSession) => void
}

export default function AuthPage({
  onAuthenticated,
}: AuthPageProps) {
  const [mode, setMode] = useState<'login' | 'register'>('login')
  const [fullName, setFullName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (loading) return

    setLoading(true)
    setError('')

    try {
      const endpoint = mode === 'login'
        ? '/api/auth/login'
        : '/api/auth/register'

      const payload = mode === 'login'
        ? { email: email.trim(), password }
        : { fullName: fullName.trim(), email: email.trim(), password }

      const response = await fetch(endpoint, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      })

      if (!response.ok) {
        if (response.status === 401) {
          throw new Error('E-mail ou senha incorretos.')
        }

        if (response.status === 400) {
          const details: unknown = await response.json()

          if (
            typeof details === 'object' &&
            details !== null &&
            'errors' in details &&
            Array.isArray(details.errors)
          ) {
            const messages = details.errors.filter(
              (item): item is string => typeof item === 'string'
            )

            if (messages.length > 0) {
              throw new Error(messages.join(' '))
            }
          }

          throw new Error('Confira os dados informados.')
        }

        throw new Error(`Erro na API: HTTP ${response.status}`)
      }

      const data: AuthSession = await response.json()

      if (!data.token || !data.userId) {
        throw new Error('Resposta de autenticação inválida.')
      }

      onAuthenticated(data)
    } catch (err) {
      setError(
        err instanceof Error ? err.message : 'Erro inesperado.'
      )
    } finally {
      setLoading(false)
    }
  }

  function changeMode() {
    if (loading) return
    setMode(previous => previous === 'login' ? 'register' : 'login')
    setError('')
    setPassword('')
  }

  return (
    <div className="auth-page">
      <div className="auth-panel">
        <div className="auth-logo">E</div>
        <h1>EduAgentAI</h1>
        <p className="auth-subtitle">
          Seu assistente inteligente para aprender idiomas.
        </p>

        <div className="auth-card">
          <h2>
            {mode === 'login' ? 'Bem-vindo de volta' : 'Criar minha conta'}
          </h2>
          <p className="auth-description">
            {mode === 'login'
              ? 'Entre para continuar seus estudos.'
              : 'Cadastre-se para começar a aprender com IA.'}
          </p>

          <form onSubmit={handleSubmit}>
            {mode === 'register' && (
              <label className="auth-field">
                Nome completo
                <input
                  type="text"
                  value={fullName}
                  onChange={event => setFullName(event.target.value)}
                  placeholder="Seu nome"
                  autoComplete="name"
                  required
                  maxLength={100}
                  disabled={loading}
                />
              </label>
            )}

            <label className="auth-field">
              E-mail
              <input
                type="email"
                value={email}
                onChange={event => setEmail(event.target.value)}
                placeholder="seuemail@exemplo.com"
                autoComplete="email"
                required
                disabled={loading}
              />
            </label>

            <label className="auth-field">
              Senha
              <input
                type="password"
                value={password}
                onChange={event => setPassword(event.target.value)}
                placeholder="Digite sua senha"
                autoComplete={
                  mode === 'login' ? 'current-password' : 'new-password'
                }
                minLength={mode === 'register' ? 8 : undefined}
                required
                disabled={loading}
              />
            </label>

            {error && (
              <div className="auth-error" role="alert">{error}</div>
            )}

            <button
              className="auth-submit"
              type="submit"
              disabled={loading}
            >
              {loading
                ? 'Aguarde...'
                : mode === 'login' ? 'Entrar' : 'Criar conta'}
            </button>
          </form>

          <p className="auth-switch">
            {mode === 'login'
              ? 'Ainda não tem uma conta?'
              : 'Já possui uma conta?'}{' '}
            <button
              type="button"
              onClick={changeMode}
              disabled={loading}
            >
              {mode === 'login' ? 'Cadastre-se' : 'Entrar'}
            </button>
          </p>
        </div>
      </div>
    </div>
  )
}
