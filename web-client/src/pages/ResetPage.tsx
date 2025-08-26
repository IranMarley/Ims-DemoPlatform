import { useState } from 'react'
import { api } from '@/lib/http'
import PageHeader from '@/components/layout/PageHeader'
import Label from '@/components/ui/label'
import Input from '@/components/ui/input'
import Button from '@/components/ui/button'
import { toast } from 'sonner'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'

export default function ResetPage(){
  const [email, setEmail] = useState('')
  const [token, setToken] = useState('')
  const [password, setPassword] = useState('')
  const [msg, setMsg] = useState('')

  return (
    <div className='max-w-md mx-auto'>
      <Card>
        <CardHeader><CardTitle>Set a new password</CardTitle></CardHeader>
        <CardContent className='space-y-3'>
          <div><Label>Email</Label><Input value={email} onChange={e=>setEmail(e.target.value)} /></div>
          <div><Label>Token</Label><Input value={token} onChange={e=>setToken(e.target.value)} /></div>
          <div><Label>New password</Label><Input type='password' value={password} onChange={e=>setPassword(e.target.value)} /></div>
          <Button className='w-full' onClick={async()=>{
            const r = await api.post('/auth/reset',{ email, token, newPassword: password }); setMsg(JSON.stringify(r.data)); toast.success('Password reset')
          }}>Reset</Button>
          {msg && <pre className='text-xs bg-slate-100 p-2 rounded'>{msg}</pre>}
        </CardContent>
      </Card>
    </div>
  )
}
