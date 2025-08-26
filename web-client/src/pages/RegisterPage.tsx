import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { useAuth } from '@/features/auth/AuthContext'
import { useState } from 'react'
import Label from '@/components/ui/label'
import Input from '@/components/ui/input'
import Button from '@/components/ui/button'
import { toast } from 'sonner'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'

const schema = z.object({ email: z.string().email(), password: z.string().min(8) })
type Form = z.infer<typeof schema>

export default function RegisterPage(){
  const { register: registerUser } = useAuth()
  const [msg, setMsg] = useState<string>('')
  const [err, setErr] = useState<string>('')
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<Form>({ resolver: zodResolver(schema) })
  return (
    <div className='max-w-md mx-auto'>
      <Card>
        <CardHeader><CardTitle>Create account</CardTitle></CardHeader>
        <CardContent>
          {msg && <div className='mb-3 text-green-700 text-sm'>{msg}</div>}
          {err && <div className='mb-3 text-red-600 text-sm'>{err}</div>}
          <form onSubmit={handleSubmit(async d => {
            setErr(''); setMsg('')
            try { await registerUser(d.email, d.password); setMsg('User created. Check your e-mail for confirmation!.'); toast.success('User created') }
            catch (e: any) { setErr(e?.message ?? 'Register failed'); toast.error('Register failed') }
          })} className='space-y-3'>
            <div>
              <Label>Email</Label>
              <Input {...register('email')} />
              {errors.email && <p className='text-xs text-red-600'>{errors.email.message}</p>}
            </div>
            <div>
              <Label>Password</Label>
              <Input type='password' {...register('password')} />
              {errors.password && <p className='text-xs text-red-600'>{errors.password.message}</p>}
            </div>
            <Button disabled={isSubmitting} className='w-full'>{isSubmitting?'…':'Create account'}</Button>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
