import { jwtDecode } from 'jwt-decode'

type JwtPayload = { role?: string | string[] } & Record<string, any>
const MS_ROLE = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

export function getRolesFromToken(token?: string | null): string[] {
  if (!token) return []
  try {
    const decoded = jwtDecode<JwtPayload>(token)
    const roles = decoded.role ?? decoded[MS_ROLE]
    if (!roles) return []
    return Array.isArray(roles) ? roles : [roles]
  } catch { return [] }
}
