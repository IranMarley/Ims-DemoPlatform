import { LabelHTMLAttributes } from 'react'
import { cn } from '@/lib/cn'

export default function Label({ className, ...props }: LabelHTMLAttributes<HTMLLabelElement>){
  return <label className={cn('label', className)} {...props} />
}
