import * as React from "react"
import { Plus, X } from "lucide-react"
import { cn } from "@/lib/utils"
import { Button } from "@/app/components/ui/button"

export interface FABAction {
  icon: React.ElementType
  label: string
  onClick: () => void
  variant?: "default" | "secondary" | "destructive"
  tooltip?: string
}

interface FloatingActionButtonProps {
  actions?: FABAction[]
  mainIcon?: React.ElementType
  mainLabel?: string
  mainTooltip?: string
  onMainClick?: () => void
  position?: "bottom-right" | "bottom-left" | "top-right" | "top-left"
  className?: string
}

/**
 * Floating Action Button Component
 *
 * A fixed-position button that can expand to show multiple action options
 * Perfect for quick access to primary actions on a page
 */
export function FloatingActionButton({
  actions = [],
  mainIcon: MainIcon = Plus,
  mainLabel,
  mainTooltip,
  onMainClick,
  position = "bottom-right",
  className,
}: FloatingActionButtonProps) {
  const [isOpen, setIsOpen] = React.useState(false)
  const [hoveredButton, setHoveredButton] = React.useState<'main' | number | null>(null)

  const positionClasses = {
    "bottom-right": "bottom-12 end-12",
    "bottom-left": "bottom-12 start-12",
    "top-right": "top-12 end-12",
    "top-left": "top-12 start-12",
  }

  const handleMainClick = () => {
    if (actions.length > 0) {
      setIsOpen(!isOpen)
    } else if (onMainClick) {
      onMainClick()
    }
  }

  const handleActionClick = (action: FABAction) => {
    action.onClick()
    setIsOpen(false)
  }

  return (
    <div className={cn("fixed z-50 flex flex-col items-end fab-bounce", positionClasses[position], className)}>
      {/* Action Buttons - appear above main button with absolute positioning */}
      {isOpen && actions.length > 0 && (
        <div className="absolute bottom-full mb-4 end-0 flex flex-col gap-3">
          {actions.map((action, index) => {
            const Icon = action.icon
            return (
              <div
                key={index}
                className="flex items-center gap-3 justify-end animate-in fade-in slide-in-from-bottom-2"
                style={{
                  animationDelay: `${index * 50}ms`,
                  animationFillMode: "backwards",
                }}
              >
                {/* Label tooltip */}
                <div className="bg-card text-card-foreground px-3 py-2 rounded-lg shadow-lg border text-sm font-medium whitespace-nowrap">
                  <div className="font-semibold">{action.label}</div>
                  {action.tooltip && (
                    <div className="text-xs text-muted-foreground mt-0.5">
                      {action.tooltip}
                    </div>
                  )}
                </div>

                {/* Action button */}
                <Button
                  size="lg"
                  variant={action.variant || "default"}
                  className="h-12 w-12 rounded-full shadow-lg hover:shadow-xl transition-all shrink-0"
                  onClick={() => handleActionClick(action)}
                  onMouseEnter={() => setHoveredButton(index)}
                  onMouseLeave={() => setHoveredButton(null)}
                >
                  <Icon className="h-5 w-5" />
                </Button>
              </div>
            )
          })}
        </div>
      )}

      {/* Main FAB Button with Tooltip */}
      <div className="relative flex items-center gap-3 shrink-0">
        {/* Tooltip for main button - only show when not open and tooltip is provided */}
        {!isOpen && mainTooltip && hoveredButton === 'main' && !mainLabel && (
          <div className="bg-card text-card-foreground px-3 py-2 rounded-lg shadow-lg border text-sm font-medium whitespace-nowrap animate-in fade-in slide-in-from-end-2">
            {mainTooltip}
          </div>
        )}

        <Button
          size="lg"
          className={cn(
            "h-14 w-14 rounded-full shadow-lg hover:shadow-xl transition-all hover:scale-110 active:scale-95",
            mainLabel && "w-auto px-6 gap-2",
            isOpen && "rotate-45"
          )}
          onClick={handleMainClick}
          onMouseEnter={() => setHoveredButton('main')}
          onMouseLeave={() => setHoveredButton(null)}
        >
          {isOpen && actions.length > 0 ? (
            <X className="h-6 w-6" />
          ) : (
            <>
              <MainIcon className="h-6 w-6" />
              {mainLabel && <span className="font-medium">{mainLabel}</span>}
            </>
          )}
        </Button>
      </div>

      {/* Backdrop overlay when open */}
      {isOpen && (
        <div
          className="fixed inset-0 -z-10 bg-background/80 backdrop-blur-sm"
          onClick={() => setIsOpen(false)}
        />
      )}
    </div>
  )
}
