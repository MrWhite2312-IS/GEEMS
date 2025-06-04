<template>
  <div class="min-h-screen p-6 w-full flex flex-col bg-gray-100">
    <!-- Mensaje de carga -->
    <div v-if="loading" class="text-center text-xl mt-10">
      Cargando datos del empleado...
    </div>
    
    <!-- Mensaje de error -->
    <div v-else-if="error" class="text-center text-red-600 mt-10">
      <p class="text-xl">Error al cargar los datos.</p>
      <button @click="fetchEmployeeData" class="mt-4 px-6 py-3 bg-blue-600 text-white rounded">
        Reintentar
      </button>
    </div>
    
    <!-- Formulario de edición -->
    <div v-else class="bg-white shadow-lg rounded-lg p-10 w-full max-w-6xl mx-auto">
      <h1 class="text-3xl font-bold mb-6">Editar perfil del empleado</h1>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <!-- Columna Izquierda -->
        <div>
          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Cédula</label>
            <input
              v-model="editableEmployee.cedulaPersona"
              disabled
              class="w-full p-2 border rounded bg-gray-100"
            />
          </div>
          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Contrato</label>
            <select v-model="editableEmployee.contrato" class="w-full p-2 border rounded">
              <option value="Tiempo Completo">Tiempo Completo</option>
              <option value="Medio Tiempo">Medio Tiempo</option>
              <option value="Servicios Profesionales">Servicios Profesionales</option>
              <option value="Por Horas">Por Horas</option>
            </select>
          </div>
          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Número de Horas Trabajadas</label>
            <input
              v-model.number="editableEmployee.numHorasTrabajadas"
              type="number"
              class="w-full p-2 border rounded"
            />
          </div>
          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Género</label>
            <select v-model="editableEmployee.genero" class="w-full p-2 border rounded">
              <option value="M">M</option>
              <option value="F">F</option>
            </select>
          </div>
        </div>
        <!-- Columna Derecha -->
        <div>
          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Estado Laboral</label>
            <select v-model="editableEmployee.estadoLaboral" class="w-full p-2 border rounded">
              <option value="Activo">Activo</option>
              <option value="Inactivo">Inactivo</option>
            </select>
          </div>
          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Salario Bruto</label>
            <input
              v-model.number="editableEmployee.salarioBruto"
              type="number"
              class="w-full p-2 border rounded"
            />
          </div>
          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Tipo</label>
            <select v-model="editableEmployee.tipo" class="w-full p-2 border rounded">
              <option value="Colaborador">Colaborador</option>
              <option value="Supervisor">Supervisor</option>
              <option value="Payroll">Payroll</option>
            </select>
          </div>
          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Nombre de la Empresa</label>
            <input
              v-model="editableEmployee.nombreEmpresa"
              disabled
              class="w-full p-2 border rounded bg-gray-100"
            />
          </div>
        </div>
      </div>
      
      <div class="mt-8 flex justify-end gap-x-4">
        <button
          @click="cancelEditing"
          class="px-6 py-3 bg-gray-500 text-white rounded hover:bg-gray-600"
        >
          Cancelar
        </button>
        <button
          @click="saveChanges"
          class="px-6 py-3 bg-blue-600 text-white rounded hover:bg-blue-700"
        >
          Guardar cambios
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import axios from "axios";
import { useUserStore } from "../store/user";

const route = useRoute();
const router = useRouter();
const userStore = useUserStore();

const cedula = route.params.cedula;
const loading = ref(true);
const error = ref(false);

const editableEmployee = ref({
  cedulaPersona: "",
  contrato: "",
  numHorasTrabajadas: 0,
  genero: "M",
  estadoLaboral: "",
  salarioBruto: 0,
  tipo: "",
  fechaIngreso: "",
  nombreEmpresa: ""
});

const fetchEmployeeData = async () => {
  loading.value = true;
  error.value = false;
  try {
    await userStore.fetchEmpleado(cedula);
    const empleadoData = userStore.empleado || {};

    editableEmployee.value = {
      cedulaPersona: cedula,
      contrato: empleadoData.contrato || "Tiempo Completo", 
      numHorasTrabajadas: empleadoData.numHorasTrabajadas || 40,
      genero: empleadoData.genero || "M",
      estadoLaboral: empleadoData.estadoLaboral || "Activo",
      salarioBruto: empleadoData.salarioBruto || 1500000,
      tipo: empleadoData.tipo || "Colaborador",
      fechaIngreso: empleadoData.fechaIngreso || "2023-01-15",
      nombreEmpresa: empleadoData.nombreEmpresa || "GEEMS Solutions"
    };
  } catch (err) {
    console.error("Error al obtener los datos del empleado", err);
    error.value = true;
  } finally {
    loading.value = false;
  }
};

const saveChanges = async () => {
  try {
    loading.value = true;
    // Validación básica de datos clave
    if (!editableEmployee.value.cedulaPersona || !editableEmployee.value.nombreEmpresa) {
      throw new Error("Faltan datos clave en la actualización.");
    }
    const empleadoUpdate = {
      cedulaPersona: Number(editableEmployee.value.cedulaPersona),
      contrato: editableEmployee.value.contrato,
      numHorasTrabajadas: Number(editableEmployee.value.numHorasTrabajadas),
      genero: editableEmployee.value.genero,
      estadoLaboral: editableEmployee.value.estadoLaboral,
      salarioBruto: Number(editableEmployee.value.salarioBruto),
      tipo: editableEmployee.value.tipo,
      fechaIngreso: editableEmployee.value.fechaIngreso,
      nombreEmpresa: editableEmployee.value.nombreEmpresa
    };

    console.log("Datos a enviar:", empleadoUpdate);
    await axios.post(`https://localhost:7014/api/Empleados/editarEmpleado`, empleadoUpdate);
    await userStore.fetchEmpleado(cedula);
    alert("Cambios guardados exitosamente");
    router.push(`/employee/${cedula}`);
  } catch (err) {
    console.error("Error al guardar los cambios:", err);
    alert("Error al guardar los cambios, verifica los datos ingresados.");
  } finally {
    loading.value = false;
  }
};

const cancelEditing = () => {
  if (confirm("¿Estás seguro de que deseas descartar los cambios?")) {
    router.go(-1);
  }
};

onMounted(() => {
  if (!userStore.usuario || !userStore.usuario.cedulaPersona) {
    window.location.href = "/";
  } else {
    fetchEmployeeData();
  }
});
</script>

<style scoped>
.form-group {
  margin-bottom: 1rem;
}
input,
select {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 4px;
}
@media (max-width: 640px) {
  .grid-cols-2 {
    grid-template-columns: 1fr;
  }
}
</style>
