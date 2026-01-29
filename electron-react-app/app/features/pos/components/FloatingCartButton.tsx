import { useCartStore } from '../stores/cartStore';
import { Badge } from '@/app/components/ui/badge';
import { CartDialog } from './CartDialog';
import { Button } from '@/app/components/ui/button';
import { ShoppingCart } from 'lucide-react';

export function FloatingCartButton() {
    const getTotalItems = useCartStore(state => state.getTotalItems);
    const itemCount = getTotalItems();

    if (itemCount === 0) return null;

    return (
        <div className="fixed bottom-6 left-6 z-50">
            <CartDialog>
                <Button
                    size="lg"
                    className="h-16 w-16 rounded-full shadow-lg hover:shadow-xl transition-all relative"
                >
                    <ShoppingCart className="h-6 w-6" />
                    <Badge
                        variant="destructive"
                        className="absolute -top-2 -right-2 h-7 w-7 flex items-center justify-center p-0 rounded-full"
                    >
                        {itemCount}
                    </Badge>
                </Button>
            </CartDialog>
        </div>
    );
}
