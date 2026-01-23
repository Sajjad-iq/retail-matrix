import React from 'react'
import ReactDOM from 'react-dom/client'
import { QueryClientProvider } from '@tanstack/react-query'
import appIcon from '@/resources/build/icon.png'
import { WindowContextProvider, menuItems } from '@/app/components/window'
import { ErrorBoundary } from '@/app/components/ErrorBoundary'
import { queryClient } from '@/app/lib/config/query'
import App from './app'

ReactDOM.createRoot(document.getElementById('app') as HTMLElement).render(
  <React.StrictMode>
    <QueryClientProvider client={queryClient}>
      <ErrorBoundary>
        <WindowContextProvider titlebar={{ title: 'Electron React App', icon: appIcon, menuItems }}>
          <App />
        </WindowContextProvider>
      </ErrorBoundary>
    </QueryClientProvider>
  </React.StrictMode>
)
