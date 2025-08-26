import { ReactNode } from 'react'
import { cn } from '@/lib/cn'

export function Card({ children, className }: { children: ReactNode, className?: string }){
  return <div className={cn('card', className)}>{children}</div>
}

export function CardHeader({ children }: { children: ReactNode }){ return <div className='mb-3'>{children}</div> }
export function CardTitle({ children }: { children: ReactNode }){ return <h3 className='text-lg font-semibold'>{children}</h3> }
export function CardContent({ children }: { children: ReactNode }){ return <div>{children}</div> }
