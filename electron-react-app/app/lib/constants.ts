import { UnitOfMeasure } from '@/app/features/products/lib/types';

export const UNIT_LABELS: Record<UnitOfMeasure, string> = {
    [UnitOfMeasure.Piece]: 'قطعة',
    [UnitOfMeasure.Dozen]: 'درزن',
    [UnitOfMeasure.Pair]: 'زوج',
    [UnitOfMeasure.Set]: 'طقم',
    [UnitOfMeasure.Kilogram]: 'كغم',
    [UnitOfMeasure.Gram]: 'غرام',
    [UnitOfMeasure.Milligram]: 'ملغم',
    [UnitOfMeasure.MetricTon]: 'طن',
    [UnitOfMeasure.Pound]: 'باوند',
    [UnitOfMeasure.Ounce]: 'أونصة',
    [UnitOfMeasure.Liter]: 'لتر',
    [UnitOfMeasure.Milliliter]: 'مل',
    [UnitOfMeasure.CubicMeter]: 'م³',
    [UnitOfMeasure.CubicCentimeter]: 'سم³',
    [UnitOfMeasure.Gallon]: 'غالون',
    [UnitOfMeasure.Quart]: 'كوارت',
    [UnitOfMeasure.Pint]: 'باينت',
    [UnitOfMeasure.FluidOunce]: 'أونصة سائلة',
    [UnitOfMeasure.Meter]: 'متر',
    [UnitOfMeasure.Centimeter]: 'سم',
    [UnitOfMeasure.Millimeter]: 'مم',
    [UnitOfMeasure.Kilometer]: 'كم',
    [UnitOfMeasure.Foot]: 'قدم',
    [UnitOfMeasure.Inch]: 'بوصة',
    [UnitOfMeasure.Yard]: 'ياردة',
    [UnitOfMeasure.SquareMeter]: 'م²',
    [UnitOfMeasure.SquareFoot]: 'قدم²',
    [UnitOfMeasure.SquareCentimeter]: 'سم²',
    [UnitOfMeasure.Box]: 'صندوق',
    [UnitOfMeasure.Pack]: 'علبة',
    [UnitOfMeasure.Bag]: 'كيس',
    [UnitOfMeasure.Bottle]: 'زجاجة',
    [UnitOfMeasure.Can]: 'علبة معدنية',
    [UnitOfMeasure.Jar]: 'مرطبان',
    [UnitOfMeasure.Tube]: 'أنبوب',
    [UnitOfMeasure.Roll]: 'رول',
    [UnitOfMeasure.Sheet]: 'ورقة',
    [UnitOfMeasure.Pallet]: 'طبلية',
    [UnitOfMeasure.Crate]: 'صندوق خشبي',
    [UnitOfMeasure.Bundle]: 'حزمة',
    [UnitOfMeasure.Carton]: 'كرتون',
    [UnitOfMeasure.Container]: 'حاوية',
};

export const WEIGHT_UNIT_LABELS: Partial<Record<UnitOfMeasure, string>> = {
    [UnitOfMeasure.Kilogram]: 'كغم',
    [UnitOfMeasure.Gram]: 'غرام',
    [UnitOfMeasure.Milligram]: 'ملغم',
    [UnitOfMeasure.MetricTon]: 'طن',
    [UnitOfMeasure.Pound]: 'باوند',
    [UnitOfMeasure.Ounce]: 'أونصة',
};

export const getUnitLabel = (unit: UnitOfMeasure): string => {
    return UNIT_LABELS[unit] || 'غير معروف';
};

export const getWeightUnitLabel = (unit: UnitOfMeasure): string => {
    return WEIGHT_UNIT_LABELS[unit] || '';
};
