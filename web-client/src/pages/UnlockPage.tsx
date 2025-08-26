import { useState } from 'react'
import { api } from '@/lib/http'
import { useAuth } from '@/features/auth/AuthContext'
import Input from '@/components/ui/input'
import Button from '@/components/ui/button'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'
import { toast } from 'sonner'

export default function UnlockPage() {
  const { accessToken } = useAuth()
  const [email, setEmail] = useState('user@example.com')
  const [msg, setMsg] = useState('')

  return (
    <div className='space-y-6'>
      <Card>
        <CardHeader><CardTitle>Unlock user</CardTitle></CardHeader>
        <CardContent className='flex gap-2'>
          <Input value={email} onChange={e=>setEmail(e.target.value)} placeholder='User email' />
          <Button onClick={async()=>{
            const r = await api.post('/admin/unlock',{ email },{ headers:{ Authorization:`Bearer ${accessToken}` } })
            setMsg(JSON.stringify(r.data)); toast.success('User unlocked')
          }}>Unlock</Button>
        </CardContent>
      </Card>
      {msg && <pre className='text-xs bg-slate-100 p-2 rounded'>{msg}</pre>}
    </div>
  )
}
