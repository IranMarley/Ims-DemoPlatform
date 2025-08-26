import { Navigate, Outlet } from 'react-router-dom'
import { useAuth } from '@/features/auth/AuthContext'
export default function RequireRole({ role }: { role: string }) {
  const { roles } = useAuth()
  return roles.includes(role) ? <Outlet/> : <Navigate to='/' replace />
}
