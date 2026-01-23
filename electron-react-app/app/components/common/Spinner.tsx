import { cn } from '@/lib/utils';

interface SpinnerProps {
  size?: 'sm' | 'md' | 'lg';
  className?: string;
  centered?: boolean;
}

/**
 * Spinner Component
 *
 * Reusable loading spinner with configurable size
 * Can be used inline or centered in a container
 */
export function Spinner({ size = 'md', className, centered = false }: SpinnerProps) {
  const sizeClasses = {
    sm: 'h-4 w-4 border',
    md: 'h-8 w-8 border-b-2',
    lg: 'h-12 w-12 border-b-2',
  };

  const spinner = (
    <div
      className={cn(
        'animate-spin rounded-full border-primary',
        sizeClasses[size],
        className
      )}
    />
  );

  if (centered) {
    return (
      <div className="flex items-center justify-center h-full">
        {spinner}
      </div>
    );
  }

  return spinner;
}
