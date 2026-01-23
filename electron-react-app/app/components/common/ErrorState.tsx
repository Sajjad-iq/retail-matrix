import type { LucideIcon } from 'lucide-react';
import { AlertTriangle } from 'lucide-react';
import { Button } from '@/app/components/ui/button';

interface ErrorStateProps {
  /** Error message to display */
  error: string;
  /** Optional title (default: "Error") */
  title?: string;
  /** Optional custom icon (default: AlertTriangle) */
  icon?: LucideIcon;
  /** Optional retry action */
  onRetry?: () => void;
  /** Optional retry button label (default: "Try Again") */
  retryLabel?: string;
  /** Optional variant for different error types */
  variant?: 'default' | 'compact';
}

/**
 * ErrorState Component
 *
 * Generic error state display with icon, title, message, and optional retry button
 *
 * @example
 * ```tsx
 * <ErrorState
 *   error="Failed to load currencies"
 *   onRetry={handleRetry}
 * />
 * ```
 *
 * @example
 * ```tsx
 * <ErrorState
 *   error="Connection timeout"
 *   title="Network Error"
 *   variant="compact"
 *   onRetry={handleRetry}
 *   retryLabel="Reload"
 * />
 * ```
 */
export function ErrorState({
  error,
  title = 'Error',
  icon: Icon = AlertTriangle,
  onRetry,
  retryLabel = 'Try Again',
  variant = 'default',
}: ErrorStateProps) {
  if (variant === 'compact') {
    return (
      <div className="bg-red-50 dark:bg-red-950/20 border border-red-200 dark:border-red-800 rounded-lg p-4">
        <div className="flex items-center gap-3">
          <Icon className="h-5 w-5 text-red-600 dark:text-red-400 flex-shrink-0" />
          <div className="flex-1 min-w-0">
            <p className="text-sm text-red-800 dark:text-red-200">{error}</p>
          </div>
          {onRetry && (
            <Button onClick={onRetry} variant="outline" size="sm" className="flex-shrink-0">
              {retryLabel}
            </Button>
          )}
        </div>
      </div>
    );
  }

  return (
    <div className="bg-red-50 dark:bg-red-950/20 border border-red-200 dark:border-red-800 rounded-lg p-6">
      <div className="flex items-start gap-3">
        <Icon className="h-6 w-6 text-red-600 dark:text-red-400 flex-shrink-0" />
        <div className="flex-1 min-w-0">
          <h3 className="text-lg font-medium text-red-800 dark:text-red-200 mb-1">{title}</h3>
          <p className="text-red-700 dark:text-red-300">{error}</p>
        </div>
      </div>
      {onRetry && (
        <Button onClick={onRetry} variant="outline" className="mt-4">
          {retryLabel}
        </Button>
      )}
    </div>
  );
}
