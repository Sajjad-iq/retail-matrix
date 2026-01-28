import { clsx, type ClassValue } from 'clsx'
import { twMerge } from 'tailwind-merge'
import { Price } from '@/app/lib/types/global'

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export function formatPrice(price: Price | number | string, currency: string = 'IQD'): string {
  // Handle Price object
  if (typeof price === 'object' && price !== null && 'amount' in price) {
    const numericAmount = price.amount;
    const curr = price.currency || currency;
    
    if (isNaN(numericAmount)) {
      return `0 ${curr}`;
    }
    
    return `${numericAmount.toLocaleString('en-US')} ${curr}`;
  }
  
  // Handle number or string
  const numericAmount = typeof price === 'string' ? parseFloat(price) : price;

  if (isNaN(numericAmount)) {
    return `0 ${currency}`;
  }

  return `${numericAmount.toLocaleString('en-US')} ${currency}`;
}
