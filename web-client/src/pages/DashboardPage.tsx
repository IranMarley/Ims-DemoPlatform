import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'

export default function DashboardPage(){
  return (
    <div className='space-y-6'>
      <Card>
        <CardHeader><CardTitle>Welcome</CardTitle></CardHeader>
        <CardContent>
          <p className='text-slate-600'>Try logging in as <code>admin@local</code>.</p>
        </CardContent>
      </Card>
    </div>
  )
}
