import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import App from './App'

describe('App', () => {
  it('shows the login form initially', () => {
    render(<App />)
    expect(screen.getByText('Login')).toBeInTheDocument()
    expect(screen.getByLabelText('username')).toBeInTheDocument()
    expect(screen.getByLabelText('password')).toBeInTheDocument()
  })

  it('has a login button', () => {
    render(<App />)
    expect(screen.getByText('Log in')).toBeInTheDocument()
  })
})
