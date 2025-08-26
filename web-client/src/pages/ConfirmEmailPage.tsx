import { useState } from 'react'
import { api } from '@/lib/http'
import Label from '@/components/ui/label'
import Input from '@/components/ui/input'
import Button from '@/components/ui/button'
import { toast } from 'sonner'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'

export default function ConfirmEmailPage(){
  const [userId, setUserId] = useState('')
  const [token, setToken] = useState('')
  const [msg, setMsg] = useState('')

  return (
    <div className='max-w-md mx-auto'>
      <Card>
        <CardHeader><CardTitle>Verify your account</CardTitle></CardHeader>
        <CardContent className='space-y-3'>
          <div className='w-full mt-4'><Label>User Id</Label><Input value={userId} onChange={e=>setUserId(e.target.value)} /></div>
          <div className='w-full mt-4'><Label>Token</Label><Input value={token} onChange={e=>setToken(e.target.value)} /></div>
          <Button className='w-full mt-4' onClick={async()=>{
            const r = await api.post('/auth/confirm-email',{ userId, token }); setMsg(JSON.stringify(r.data)); toast.success('Email confirmed')
          }}>Confirm</Button>
          {msg && <pre className='text-xs bg-slate-100 p-2 rounded'>{msg}</pre>}
        </CardContent>
      </Card>
    </div>
  )
}
