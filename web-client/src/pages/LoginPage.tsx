import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { useAuth } from '@/features/auth/AuthContext'
import { Link, useNavigate } from 'react-router-dom'
import { useState } from 'react'
import Label from '@/components/ui/label'
import Input from "@/components/ui/input";
import Button from '@/components/ui/button'
import { toast } from 'sonner'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'

const schema = z.object({ email: z.string().email(), password: z.string().min(6) })
type Form = z.infer<typeof schema>

export default function LoginPage(){
  const { login } = useAuth()
  const nav = useNavigate()
  const [error, setError] = useState<string | null>(null)
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<Form>({ resolver: zodResolver(schema) })

  async function onSubmit(data: Form){
    setError(null)
    try {
      await login(data.email, data.password)
      toast.success('Signed in')
      nav('/dashboard')
    } catch (e: any) {
      setError(e?.message ?? 'Login failed')
      toast.error('Login failed')
    }
  }

  return (
    <div className='max-w-md mx-auto'>
      <Card>
        <CardHeader><CardTitle>Sign in</CardTitle></CardHeader>
        <CardContent>
          {error && <div className='mb-3 text-red-600 text-sm'>{error}</div>}
          <form onSubmit={handleSubmit(onSubmit)} className='space-y-3'>
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
            <Button disabled={isSubmitting} className='w-full'>{isSubmitting?'…':'Sign in'}</Button>
          </form>
          <div className='mt-4 text-sm flex justify-between'>
            <Link to='/register' className='text-indigo-600'>Create account</Link>
            <Link to='/forgot' className='text-indigo-600'>Forgot password?</Link>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
