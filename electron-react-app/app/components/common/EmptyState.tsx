import type { LucideIcon } from 'lucide-react';
import { Button } from '@/app/components/ui/button';

interface EmptyStateProps {
  /** Icon to display */
  icon: LucideIcon;
  /** Title text */
  title: string;
  /** Description text */
  description?: string;
  /** Optional action button */
  action?: {
    label: string;
    onClick: () => void;
    icon?: LucideIcon;
  };
  /** Optional custom icon size (default: 12) */
  iconSize?: 'sm' | 'md' | 'lg' | 'xl';
}

const iconSizeClasses = {
  sm: 'h-8 w-8',
  md: 'h-10 w-10',
  lg: 'h-12 w-12',
  xl: 'h-16 w-16',
};

/**
 * EmptyState Component
 *
 * Generic empty state display with icon, title, description, and optional action button
 *
 * @example
 * ```tsx
 * <EmptyState
 *   icon={Banknote}
 *   title="No currencies found"
 *   description="Get started by adding your first currency"
 *   action={{
 *     label: "Add Currency",
 *     onClick: handleCreate,
 *     icon: Plus
 *   }}
 * />
 * ```
 */
export function EmptyState({
  icon: Icon,
  title,
  description,
  action,
  iconSize = 'lg',
}: EmptyStateProps) {
  const ActionIcon = action?.icon;

  return (
    <div className="text-center py-12">
      <Icon className={`${iconSizeClasses[iconSize]} text-muted-foreground mx-auto mb-4`} />
      <h3 className="text-lg font-medium mb-2">{title}</h3>
      {description && <p className="text-sm text-muted-foreground mb-4">{description}</p>}
      {action && (
        <Button onClick={action.onClick}>
          {ActionIcon && <ActionIcon className="h-4 w-4 me-2" />}
          {action.label}
        </Button>
      )}
    </div>
  );
}
