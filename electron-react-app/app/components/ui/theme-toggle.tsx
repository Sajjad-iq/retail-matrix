import * as React from "react"
import { Button } from "@/app/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/app/components/ui/dropdown-menu"
import { Sun, Moon, Monitor } from "lucide-react"
import { useThemeStore } from "@/stores/theme"
import { useTranslation } from "react-i18next"

export type Theme = "light" | "dark" | "system"

export interface ThemeToggleProps {
  theme?: Theme
  onThemeChange?: (theme: Theme) => void
}

export function ThemeToggle({ theme: propTheme, onThemeChange }: ThemeToggleProps) {
  const { t } = useTranslation()
  const theme = useThemeStore((state) => state.theme)
  const setTheme = useThemeStore((state) => state.setTheme)
  const getEffectiveTheme = useThemeStore((state) => state.getEffectiveTheme)

  // Support for controlled component via props
  const currentTheme = propTheme ?? theme

  // Get effective theme (resolve system theme) - only on client side
  const [effectiveTheme, setEffectiveTheme] = React.useState<"light" | "dark">("light")
  const [mounted, setMounted] = React.useState(false)

  // Avoid hydration mismatch by only rendering theme-dependent content on client
  React.useEffect(() => {
    setMounted(true)
    setEffectiveTheme(getEffectiveTheme())
  }, [getEffectiveTheme])

  // Update effective theme when current theme changes
  React.useEffect(() => {
    if (mounted) {
      setEffectiveTheme(getEffectiveTheme())
    }
  }, [currentTheme, mounted, getEffectiveTheme])

  // Get current icon - fallback to Monitor during SSR
  const CurrentIcon = React.useMemo(() => {
    if (!mounted) return Monitor
    if (currentTheme === "system") return Monitor
    return effectiveTheme === "dark" ? Moon : Sun
  }, [currentTheme, effectiveTheme, mounted])

  // Handle theme change
  const handleThemeChange = (newTheme: Theme) => {
    if (propTheme !== undefined && onThemeChange) {
      // Controlled mode
      onThemeChange(newTheme)
    } else {
      // Uncontrolled mode - use store
      setTheme(newTheme)
    }
  }

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="ghost" size="icon" className="h-9 w-9">
          <CurrentIcon className="h-4 w-4" />
          <span className="sr-only">{t("theme.toggleTheme")}</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        <DropdownMenuItem onClick={() => handleThemeChange("light")}>
          <Sun className="me-2 h-4 w-4" />
          <span>{t("theme.light")}</span>
        </DropdownMenuItem>
        <DropdownMenuItem onClick={() => handleThemeChange("dark")}>
          <Moon className="me-2 h-4 w-4" />
          <span>{t("theme.dark")}</span>
        </DropdownMenuItem>
        <DropdownMenuItem onClick={() => handleThemeChange("system")}>
          <Monitor className="me-2 h-4 w-4" />
          <span>{t("theme.system")}</span>
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  )
}
