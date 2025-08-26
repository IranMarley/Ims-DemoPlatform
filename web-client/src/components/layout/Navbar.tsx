import { Link, NavLink } from 'react-router-dom'
import { useAuth } from '@/features/auth/AuthContext'
import { Shield, LogOut, LogIn } from 'lucide-react'

export default function Navbar(){
  const { isAuthenticated, logout } = useAuth()
  return (
    <header className='border-b bg-white'>
      <div className='container flex h-14 items-center justify-between'>
        <Link to='/' className='inline-flex items-center gap-2 font-semibold'>
          <Shield className='h-5 w-5'/> Auth Admin
        </Link>
        <nav className='flex items-center gap-4 text-sm'>
          { isAuthenticated ? 
          (
          <>
            <NavLink to='/dashboard' className={({isActive}) => isActive ? 'text-slate-900' : 'text-slate-600 hover:text-slate-900'}>Dashboard</NavLink>
            <NavLink to='/roles' className={({isActive}) => isActive ? 'text-slate-900' : 'text-slate-600 hover:text-slate-900'}>Roles</NavLink>
            <NavLink to='/unlock' className={({isActive}) => isActive ? 'text-slate-900' : 'text-slate-600 hover:text-slate-900'}>Unlock</NavLink>
          </>
          ) : null}
        </nav>
        <div>
          {isAuthenticated
            ? <button onClick={logout} className='btn btn-outline'><LogOut className='h-4 w-4'/> Logout</button>
            : <Link to='/login' className='btn btn-primary'><LogIn className='h-4 w-4'/> Login</Link>}
        </div>
      </div>
    </header>
  )
}
