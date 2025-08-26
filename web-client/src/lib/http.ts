import axios from 'axios'

const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5005'
export const api = axios.create({ baseURL: API_BASE })

let isRefreshing = false
let queue: Array<(token: string | null) => void> = []

function setAuthHeader(token?: string | null) {
  if (token) api.defaults.headers.common['Authorization'] = `Bearer ${token}`
  else delete api.defaults.headers.common['Authorization']
}

export function loadTokens() {
  const access = localStorage.getItem('accessToken')
  const refresh = localStorage.getItem('refreshToken')
  if (access) setAuthHeader(access)
  return { access, refresh }
}
export function saveTokens(access: string, refresh: string) {
  localStorage.setItem('accessToken', access)
  localStorage.setItem('refreshToken', refresh)
  setAuthHeader(access)
}
export function clearTokens() {
  localStorage.removeItem('accessToken')
  localStorage.removeItem('refreshToken')
  setAuthHeader(null)
}

// 401 refresh flow
api.interceptors.response.use(
  r => r,
  async (error) => {
    const original = error.config
    if (error.response?.status === 401 && !original._retry) {
      original._retry = true
      if (isRefreshing) {
        return new Promise(resolve => {
          queue.push((token) => {
            if (token) original.headers['Authorization'] = `Bearer ${token}`
            resolve(api(original))
          })
        })
      }
      isRefreshing = true
      try {
        const refresh = localStorage.getItem('refreshToken')
        if (!refresh) throw new Error('no refresh')
        const res = await api.post('/auth/refresh', { refreshToken: refresh })
        saveTokens(res.data.accessToken, res.data.refreshToken)
        queue.forEach(cb => cb(res.data.accessToken))
        queue = []
        return api(original)
      } catch (e) {
        clearTokens()
        queue.forEach(cb => cb(null)); queue = []
        return Promise.reject(e)
      } finally {
        isRefreshing = false
      }
    }
    return Promise.reject(error)
  }
)
