<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import type { SelectedComponents, SelectedComponentsResponse } from '../types';
import { getConfig } from '../api/api';
import Modal from './modals/Modal.vue';
import ComponentCard from '../components/ComponentCard.vue';
import RecapItem from '../components/RecapItem.vue';
import Logo from '../assets/images/logo.png';

const selectedComponents = ref<SelectedComponents>({
    case: null,
    powersupply: null,
    videocard: null,
    cpuCooler: null,
    storage: null,
    motherboard: null,
    cpu: null,
    ram: null,
});
const typeComponent = ref<string>('');
const isModalOpen = ref<boolean>(false);
const configPrice = ref<number>(0);

const handleSelectedComponentsUpdate = (components: SelectedComponents) => {
    selectedComponents.value = components;
};

function updateTotalPrice(): void {
    let total = 0;
    const comps = selectedComponents.value;
    const keys: (keyof SelectedComponents)[] = [
        'case',
        'motherboard',
        'cpu',
        'cpuCooler',
        'ram',
        'videocard',
        'powersupply',
        'storage',
    ];
    keys.forEach((key) => {
        const comp = comps[key];
        if (comp && comp.price) {
            const priceStr = comp.price.replace('$', '').trim();
            const priceNum = parseFloat(priceStr);
            if (!isNaN(priceNum)) {
                total += priceNum;
            }
        }
    });
    configPrice.value = total;
    return;
}

onMounted(async () => {
    const config = <SelectedComponentsResponse>await getConfig();
    configPrice.value = config.price;
    selectedComponents.value = {
        case: config.pcCase,
        powersupply: config.powerSupply,
        videocard: config.videoCard,
        cpuCooler: config.cpuCooler,
        storage: config.storageDevice,
        motherboard: config.motherboard,
        cpu: config.cpu,
        ram: config.memory,
    };
});

watch(
    selectedComponents,
    () => {
        updateTotalPrice();
    },
    { deep: true }
);
</script>

<template>
    <div class="flex items-center justify-center space-x-8 w-full mt-10">
        <img :src="Logo" alt="Case" class="w-24" />
        <span class="text-4xl font-bold text-white">CONFIGURATEUR</span>
    </div>

    <div class="flex justify-center mt-10 space-x-8 text-white">
        <div class="flex flex-col items-start w-[1000px] space-y-2 mb-10">
            <span class="text-2xl font-semibold opacity-50">COMPOSANTS</span>
            <ComponentCard
                :componentType="'case'"
                :component="selectedComponents.case"
                :selectedComponents="selectedComponents"
                @selected-components="handleSelectedComponentsUpdate"
            />
            <ComponentCard
                :componentType="'powersupply'"
                :component="selectedComponents.powersupply"
                :selectedComponents="selectedComponents"
                @selected-components="handleSelectedComponentsUpdate"
            />
            <ComponentCard
                :componentType="'videocard'"
                :component="selectedComponents.videocard"
                :selectedComponents="selectedComponents"
                @selected-components="handleSelectedComponentsUpdate"
            />
            <ComponentCard
                :componentType="'cpu-cooler'"
                :component="selectedComponents.cpuCooler"
                :selectedComponents="selectedComponents"
                @selected-components="handleSelectedComponentsUpdate"
            />
            <ComponentCard
                :componentType="'storage'"
                :component="selectedComponents.storage"
                :selectedComponents="selectedComponents"
                @selected-components="handleSelectedComponentsUpdate"
            />
            <ComponentCard
                :componentType="'motherboard'"
                :component="selectedComponents.motherboard"
                :selectedComponents="selectedComponents"
                @selected-components="handleSelectedComponentsUpdate"
            />
            <ComponentCard
                :componentType="'cpu'"
                :component="selectedComponents.cpu"
                :selectedComponents="selectedComponents"
                @selected-components="handleSelectedComponentsUpdate"
            />
            <ComponentCard
                :componentType="'ram'"
                :component="selectedComponents.ram"
                :selectedComponents="selectedComponents"
                @selected-components="handleSelectedComponentsUpdate"
            />
        </div>

        <div class="w-[400px] text-white space-y-2">
            <span class="text-2xl font-semibold opacity-50">RECAPITULATIF</span>
            <div
                class="p-4 bg-[var(--color-background-secondary-dark)] border-2 border-[green] border-opacity-50 rounded-md"
				:class="{ 'space-y-4': configPrice !== 0 }"
            >
                <div class="space-y-1" :class="{ 'border-b-2 border-white': configPrice !== 0 }">
                    <RecapItem :component="selectedComponents.case" />
                    <RecapItem :component="selectedComponents.cpu" />
                    <RecapItem :component="selectedComponents.cpuCooler" />
                    <RecapItem :component="selectedComponents.motherboard" />
                    <RecapItem :component="selectedComponents.powersupply" />
                    <RecapItem :component="selectedComponents.storage" />
                    <RecapItem :component="selectedComponents.videocard" />
                    <RecapItem :component="selectedComponents.ram" />
                </div>

                <div class="text-lg font-bold w-full flex justify-between p-2">
                    <span>TOTAL :</span>
                    <span>${{ configPrice }}</span>
                </div>
            </div>
        </div>
    </div>

    <Modal
        v-model:isOpen="isModalOpen"
        :componentType="typeComponent"
        :selected-components="selectedComponents"
    />
</template>

<style>
::-webkit-scrollbar {
    width: 8px;
}

::-webkit-scrollbar-track {
    background: var(--color-background-dark);
}

::-webkit-scrollbar-thumb {
    background: var(--color-primary);
    border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
    background: green;
}
</style>
