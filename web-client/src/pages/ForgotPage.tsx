import { useState } from 'react'
import { api } from '@/lib/http'
import Label from '@/components/ui/label'
import Input from '@/components/ui/input'
import Button from '@/components/ui/button'
import { toast } from 'sonner'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'

export default function ForgotPage(){
  const [email, setEmail] = useState('')

  return (
    <div className='max-w-md mx-auto'>
      <Card>
        <CardHeader><CardTitle>Send reset link</CardTitle></CardHeader>
        <CardContent className='space-y-3'>
          <div><Label>Email</Label><Input value={email} onChange={e=>setEmail(e.target.value)} /></div>
          <Button className='w-full mt-4' onClick={async()=>{
            const r = await api.post('/auth/forgot',{ email }); setMsg(JSON.stringify(r.data)); toast.success('If the email exists, a reset link was sent!')
          }}>Send</Button>
        </CardContent>
      </Card>
    </div>
  )
}
