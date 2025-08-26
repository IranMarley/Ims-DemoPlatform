import { useEffect, useState } from 'react'
import { api } from '@/lib/http'
import { useAuth } from '@/features/auth/AuthContext'
import Input from '@/components/ui/input'
import Button from '@/components/ui/button'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'
import { toast } from 'sonner'

type Role = { id: string; name: string }

export default function RolesPage() {
  const { accessToken } = useAuth()
  const [roles, setRoles] = useState<Role[]>([])
  const [name, setName] = useState('Manager')
  const [email, setEmail] = useState('admin@local')
  const [output, setOutput] = useState('')

  useEffect(() => { load() }, [])

  async function load() {
    const r = await api.get('/roles', { headers: { Authorization: `Bearer ${accessToken}` } })
    setRoles(r.data)
  }

  return (
    <div className='space-y-6'>
      <Card>
        <CardHeader><CardTitle>Create / Delete Role</CardTitle></CardHeader>
        <CardContent className='flex gap-2'>
          <Input value={name} onChange={e=>setName(e.target.value)} placeholder='Role name' />
          <Button onClick={async()=>{ await api.post('/roles',{name},{headers:{Authorization:`Bearer ${accessToken}`}}); toast.success('Role created'); await load() }}>Create</Button>
          <Button variant='outline' onClick={async()=>{ await api.delete(`/roles/${name}`,{headers:{Authorization:`Bearer ${accessToken}`}}); toast.success('Role deleted'); await load() }}>Delete</Button>
        </CardContent>
      </Card>

      <Card>
        <CardHeader><CardTitle>Assign / Remove to User</CardTitle></CardHeader>
        <CardContent className='space-y-2'>
          <div className='flex gap-2'>
            <Input value={email} onChange={e=>setEmail(e.target.value)} placeholder='User email' />
            <Input value={name} onChange={e=>setName(e.target.value)} placeholder='Role name' />
            <Button onClick={async()=>{
              const r=await api.post('/roles/assign',{email,role:name},{headers:{Authorization:`Bearer ${accessToken}`}})
              setOutput(JSON.stringify(r.data)); toast.success('Role assigned')
            }}>Assign</Button>
            <Button variant='outline' onClick={async()=>{
              const r=await api.post('/roles/remove',{email,role:name},{headers:{Authorization:`Bearer ${accessToken}`}})
              setOutput(JSON.stringify(r.data)); toast.success('Role removed')
            }}>Remove</Button>
          </div>
          {output && <pre className='text-xs bg-slate-100 p-2 rounded'>{output}</pre>}
        </CardContent>
      </Card>

      <Card>
        <CardHeader><CardTitle>Existing Roles</CardTitle></CardHeader>
        <CardContent>
          <table className='table'>
            <thead><tr><th>Name</th><th>ID</th></tr></thead>
            <tbody>{roles.map(r => <tr key={r.id}><td>{r.name}</td><td className='text-slate-500'>{r.id}</td></tr>)}</tbody>
          </table>
        </CardContent>
      </Card>
    </div>
  )
}
