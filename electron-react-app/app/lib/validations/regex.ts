export const DOMAIN_REGEX = /^(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}$/;
export const PHONE_REGEX = /^\+?[1-9]\d{1,14}$/;

// Helper to remove non-digits for phone normalization before validation
export const normalizePhone = (phone: string) => phone.replace(/[\s\-\(\)]/g, '');
