import React from 'react'
import ReactDOM from 'react-dom/client'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { QueryClientProvider } from '@tanstack/react-query'
import { queryClient } from '@/lib/queryClient'
import { AuthProvider } from '@/features/auth/AuthContext'
import ProtectedRoute from '@/routes/ProtectedRoute'
import RequireRole from '@/routes/RequireRole'
import Navbar from '@/components/layout/Navbar'
import AppToaster from '@/components/ui/toaster'
import '@/styles/index.css'

import LoginPage from '@/pages/LoginPage'
import RegisterPage from '@/pages/RegisterPage'
import ConfirmEmailPage from '@/pages/ConfirmEmailPage'
import ForgotPage from '@/pages/ForgotPage'
import ResetPage from '@/pages/ResetPage'
import DashboardPage from '@/pages/DashboardPage'
import RolesPage from '@/pages/RolesPage'
import UnlockPage from '@/pages/UnlockPage'
import NotFoundPage from '@/pages/NotFoundPage'

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <BrowserRouter>
          <Navbar />
          <main className='container py-6'>
            <Routes>
              <Route path='/' element={<DashboardPage />} />
              <Route path='/login' element={<LoginPage />} />
              <Route path='/register' element={<RegisterPage />} />
              <Route path='/confirm-email' element={<ConfirmEmailPage />} />
              <Route path='/forgot' element={<ForgotPage />} />
              <Route path='/reset' element={<ResetPage />} />

              <Route element={<ProtectedRoute />}>
                <Route path='/dashboard' element={<DashboardPage />} />
                <Route element={<RequireRole role="Admin" />}>
                  <Route path='/roles' element={<RolesPage />} />
                  <Route path='/unlock' element={<UnlockPage />} />
                </Route>
              </Route>

              <Route path='*' element={<NotFoundPage />} />
            </Routes>
          </main>
          <AppToaster />
        </BrowserRouter>
      </AuthProvider>
    </QueryClientProvider>
  )
}

ReactDOM.createRoot(document.getElementById('root')!).render(<App />)
