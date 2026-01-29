import React from 'react'
import ReactDOM from 'react-dom/client'
import { QueryClientProvider } from '@tanstack/react-query'
import { ErrorBoundary } from '@/app/components/ErrorBoundary'
import { queryClient } from '@/app/lib/config/query'
import App from './app'

ReactDOM.createRoot(document.getElementById('app') as HTMLElement).render(
  <QueryClientProvider client={queryClient}>
    <ErrorBoundary>
      <App />
    </ErrorBoundary>
  </QueryClientProvider>
)
