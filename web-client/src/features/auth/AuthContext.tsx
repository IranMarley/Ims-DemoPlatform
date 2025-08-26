import React, { createContext, useContext, useEffect, useMemo, useState } from 'react'
import { api, loadTokens, saveTokens, clearTokens } from '@/lib/http'
import { getRolesFromToken } from '@/lib/jwt'

type AuthState = {
  accessToken: string | null
  refreshToken: string | null
  roles: string[]
  isAuthenticated: boolean
  login: (email: string, password: string) => Promise<void>
  register: (email: string, password: string) => Promise<void>
  logout: () => Promise<void>
}

const AuthCtx = createContext<AuthState | null>(null)

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [accessToken, setAccess] = useState<string | null>(null)
  const [refreshToken, setRefresh] = useState<string | null>(null)
  const [roles, setRoles] = useState<string[]>([])

  useEffect(() => {
    const t = loadTokens()
    setAccess(t.access || null)
    setRefresh(t.refresh || null)
    setRoles(getRolesFromToken(t.access))
  }, [])

  const value = useMemo<AuthState>(() => ({
    accessToken,
    refreshToken,
    roles,
    isAuthenticated: !!accessToken,
    login: async (email, password) => {
      const r = await api.post('/auth/login', { email, password })
      saveTokens(r.data.accessToken, r.data.refreshToken)
      setAccess(r.data.accessToken); setRefresh(r.data.refreshToken); setRoles(getRolesFromToken(r.data.accessToken))
    },
    register: async (email, password) => {
      await api.post('/auth/register', { email, password })
    },
    logout: async () => {
      try { if (refreshToken) await api.post('/auth/logout', { refreshToken }) } catch {}
      clearTokens(); setAccess(null); setRefresh(null); setRoles([])
    }
  }), [accessToken, refreshToken, roles])

  return <AuthCtx.Provider value={value}>{children}</AuthCtx.Provider>
}

export function useAuth() {
  const ctx = useContext(AuthCtx)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
