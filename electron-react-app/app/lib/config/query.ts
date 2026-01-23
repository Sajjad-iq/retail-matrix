import { QueryClient } from '@tanstack/react-query';

// Cache duration constants
const STALE_TIME = 3 * 60 * 1000; // 3 minutes - data considered fresh
const GC_TIME = 5 * 60 * 1000; // 5 minutes - garbage collection time

/**
 * TanStack Query Client Configuration
 *
 * Settings optimized for the application:
 * - 3 minute stale time (TTL - data considered fresh)
 * - 5 minute garbage collection time
 * - 2 retries for failed queries
 * - No automatic refetching (manual control)
 */
export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: STALE_TIME,
      gcTime: GC_TIME,
      retry: 3,
      refetchOnWindowFocus: false,
      refetchOnReconnect: false,
      refetchOnMount: false,
    },
    mutations: {
      retry: 1,
    },
  },
});

/**
 * Query Utility Functions
 */
export const queryUtils = {

  /**
 * Clear all queries (used on logout)
   */
  clearAll: () => {
    queryClient.clear();
  },

  /**
   * Reset all queries (refetch all active queries)
   */
  resetAll: () => {
    queryClient.resetQueries();
  },
};
