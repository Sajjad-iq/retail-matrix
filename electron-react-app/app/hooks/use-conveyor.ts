type ConveyorKey = keyof Window['conveyor']

// Check if running in Electron (conveyor is exposed via preload)
const isElectron = typeof window !== 'undefined' && window.conveyor !== undefined

/**
 * Use the conveyor for inter-process communication
 *
 * @param key - The key of the conveyor object to use
 * @returns The conveyor object or the keyed object (undefined in browser)
 */
export const useConveyor = <T extends ConveyorKey | undefined = undefined>(
  key?: T
): T extends ConveyorKey ? Window['conveyor'][T] | undefined : Window['conveyor'] | undefined => {
  if (!isElectron) {
    return undefined as any
  }

  const conveyor = window.conveyor

  if (key) {
    return conveyor[key] as any
  }

  return conveyor as any
}
