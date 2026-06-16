import { useState } from 'react'
import './index.css'

// The API base URL. In production this gets set to your deployed API.
// For local dev it points to the .NET API running on localhost.
const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

function App() {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [token, setToken] = useState(null)
  const [loggedInUser, setLoggedInUser] = useState('')
  const [secret, setSecret] = useState('')
  const [error, setError] = useState('')

  const handleLogin = async () => {
    setError('')
    try {
      const res = await fetch(`${API_URL}/api/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password }),
      })
      if (!res.ok) {
        setError('Invalid username or password')
        return
      }
      const data = await res.json()
      setToken(data.token)
      setLoggedInUser(data.username)
    } catch {
      setError('Could not reach the API. Is it running?')
    }
  }

  const fetchSecret = async () => {
    setError('')
    try {
      const res = await fetch(`${API_URL}/api/secret`, {
        headers: { Authorization: `Bearer ${token}` },
      })
      if (!res.ok) {
        setError('Not authorized')
        return
      }
      const data = await res.json()
      setSecret(data.message)
    } catch {
      setError('Could not reach the API')
    }
  }

  const handleLogout = () => {
    setToken(null)
    setLoggedInUser('')
    setSecret('')
    setUsername('')
    setPassword('')
  }

  // Logged-in view
  if (token) {
    return (
      <div className="app">
        <h1>Welcome, {loggedInUser} 👋</h1>
        <p className="subtitle">You are logged in.</p>
        <button onClick={fetchSecret}>Get protected data</button>
        {secret && <p className="result">{secret}</p>}
        {error && <p className="error">{error}</p>}
        <button className="logout" onClick={handleLogout}>Log out</button>
      </div>
    )
  }

  // Login view
  return (
    <div className="app">
      <h1>Login</h1>
      <p className="subtitle">Try admin / password123</p>
      <input
        placeholder="Username"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        aria-label="username"
      />
      <input
        type="password"
        placeholder="Password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        aria-label="password"
      />
      <button onClick={handleLogin}>Log in</button>
      {error && <p className="error">{error}</p>}
    </div>
  )
}

export default App
