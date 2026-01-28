import { Button } from '@/app/components/ui/button';
import { ShoppingCart } from 'lucide-react';
import { useCartStore } from '../stores/cartStore';
import { useDraftSale } from '../hooks/usePosActions';
import { Badge } from '@/app/components/ui/badge';

interface FloatingCartButtonProps {
    onClick: () => void;
}

export function FloatingCartButton({ onClick }: FloatingCartButtonProps) {
    const inventoryId = useCartStore(state => state.inventoryId);
    const { data: draftSale } = useDraftSale(inventoryId);

    const itemCount = draftSale?.totalItems || 0;

    if (itemCount === 0) return null;

    return (
        <div className="fixed bottom-6 left-6 z-50">
            <Button
                size="lg"
                className="h-16 w-16 rounded-full shadow-lg hover:shadow-xl transition-all relative"
                onClick={onClick}
            >
                <ShoppingCart className="h-6 w-6" />
                <Badge
                    variant="destructive"
                    className="absolute -top-2 -right-2 h-7 w-7 flex items-center justify-center p-0 rounded-full"
                >
                    {itemCount}
                </Badge>
            </Button>
        </div>
    );
}
