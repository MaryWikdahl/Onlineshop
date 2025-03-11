<template>
  <div class="admin-orders">
    <h1>Admin Order Management</h1>
   <!-- Search for Specific Order -->
   <div>
      <input
        v-model="searchOrderId"
        type="number"
        placeholder="Search Order by ID"
      />
      <button @click="fetchOrderById">Search</button>
    </div>
    <!-- Fetch All Orders -->
    <div>
      <button @click="fetchOrders">Fetch All Orders</button>
    </div>


    <!-- Orders Table -->
    <div v-if="orders.length > 0 || selectedOrder || paginatedProducts.length && IsTrue ">
         <!-- Sort Orders by Status -->
   <div>
      <label for="statusFilter">Filter by Status:</label>
      <select v-model="statusFilter" @change="filterOrders">
        <option value="">All</option>
        <option v-for="status in statuses" :key="status" :value="status">
          {{ status }}
        </option>
      </select>
    </div>
<div>

  <!-- Sort by Order Date -->
  <label for="dateSort">Sort by Date:</label>
  <select v-model="dateSortOrder" @change="sortAndPaginate">
    <option value="desc">Newest to Oldest</option>
    <option value="asc">Oldest to Newest</option>
  </select>

  </div>
      <h2>Orders</h2>
      <table>
        
        <!-- sidhantering -->
    <div class="pagination">
      <button class="action-button main-button" @click="changePage(currentPage - 1)" :disabled="currentPage === 1">
        Föregående
      </button>
    <span>Sida {{ currentPage }} av {{ totalPages }}</span>
    <button class="action-button main-button" @click="changePage(currentPage + 1)" :disabled="currentPage === totalPages">
        Nästa
    </button>
    </div>
        <thead>
          <tr>
            <th>Order ID</th>
            <th>Order Date</th>
            <th>Total Amount</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="order in paginatedOrders"
            :key="order.orderId"
            :class="{ selected: selectedOrder?.orderId === order.orderId }"
          >
            <td>{{ order.orderId }}</td>
            <td>{{ new Date(order.orderDate).toLocaleDateString() }}</td>
            <td>{{ order.totalAmount.toFixed(2) }}</td>
            <td>
              <select v-model="order.status" @change="updateOrderStatus(order)">
                <option v-for="status in statuses" :key="status" :value="status">
                  {{ status }}
                </option>
              </select>
            </td>
            <td>
              <!-- Only allow actions for "Pending" orders -->
              <button v-if="order.status === 'Pending'" @click="fetchOrderDetails(order.orderId)">Edit</button>
              <button v-if="order.status === 'Pending'" @click="deleteOrder(order.orderId)">Delete</button>
              <button v-else disabled class="disable-btn">Cannot edit or delete</button> <!-- Disabled button for non-pending orders -->
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Selected Order Details -->
    <div v-if="selectedOrder">
      <h2>Edit Order (ID: {{ selectedOrder.orderId }})</h2>
      <div>
        <h3>Order Items:</h3>
        <table>
          <thead>
            <tr>
              <th>Product ID</th>
              <th>Quantity</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in selectedOrder.orderItems" :key="item.productId">
              <td>{{ item.productId }}</td>
              <td>
                <input
                  type="number"
                  v-model.number="item.quantity"
                  min="1"
                  @change="validateQuantity(item)"
                />
              </td>
            </tr>
          </tbody>
        </table>
        <button @click="updateOrder">Save Changes</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      orders: [], // List of all orders
      selectedOrder: null, // Order being edited
      searchOrderId: "", // ID for search
      statuses: ["Pending", "Shipped", "Completed", "Cancelled"], // Possible statuses
      statusFilter: "", // Filter status for the orders
      dateSortOrder: "desc",
      currentPage: 1,
      paginatedProducts: [],
    pageSize: 10,

    };
  },
computed: {
  filteredOrders() {
    let filtered = this.orders;

    // Filter by status if it's set
    if (this.statusFilter) {
      filtered = filtered.filter(order => order.status === this.statusFilter);
    }

    // Sort by order date (ascending or descending)
    filtered.sort((a, b) => {
      const dateA = new Date(a.orderDate);
      const dateB = new Date(b.orderDate);

      // Sorting logic: ascending or descending
      if (this.dateSortOrder === "asc") {
        return dateA - dateB; // Oldest to Newest
      } else {
        return dateB - dateA; // Newest to Oldest
      }
    });

    return filtered;
  },

  paginatedOrders() {
    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;
    return this.filteredOrders.slice(start, end);
  },

  totalPages() {
    return Math.ceil(this.filteredOrders.length / this.pageSize);
  }
},
  methods: {
    changePage(newPage) {
    if (newPage > 0 && newPage <= this.totalPages) {
      this.currentPage = newPage;
      this.sortAndPaginate();
    }
  },
  sortAndPaginate() {
  // Sortera produkter baserat på id
  let sortedProducts = [...this.orders];
  sortedProducts.sort((a, b) => {
    return this.sortOrder === "desc" ? b.id - a.id : a.id - b.id;
  });

  // Paginera produkter
  const start = (this.currentPage - 1) * this.pageSize;
  const end = start + this.pageSize;
  this.paginatedProducts = sortedProducts.slice(start, end);
},

  changeSortOrder(order) {
    this.sortOrder = order;
    this.sortAndPaginate();
  },


    // Fetch all orders
    async fetchOrders() {
      try {
        const response = await fetch("https://localhost:7131/api/orders", {
          method: "GET",
          headers: {
            "Authorization": `Bearer ${this.$store.state.token}`, // Token for authentication
            "Content-Type": "application/json",
          },
        });
        if (!response.ok) throw new Error("Failed to fetch orders.");
        this.orders = await response.json();
        this.selectedOrder = null; // Clear any selected order
        this.totalPages = Math.ceil(this.orders.length / this.pageSize);
        this.sortAndPaginate();
      } catch (error) {
        console.error(error);
        alert(error.message);
      }
    },

    // Fetch a specific order by ID
    async fetchOrderById() {
      if (!this.searchOrderId) {
        alert("Please enter an order ID.");
        return;
      }
      try {
        const response = await fetch(`https://localhost:7131/api/orders/${String(this.searchOrderId)}`, {
          method: "GET",
          headers: {
            "Authorization": `Bearer ${this.$store.state.token}`,
          },
        });

        if (!response.ok) throw new Error("Order not found.");
        this.selectedOrder = await response.json();
        console.log(this.selectedOrder);
      } catch (error) {
        console.error(error);
        alert(error.message);
      }
    },

    // Fetch details of an order for editing
    async fetchOrderDetails(orderId) {
      try {
        const response = await fetch(`https://localhost:7131/api/orders/${orderId}`, {
          method: "GET",
          headers: {
            "Authorization": `Bearer ${this.$store.state.token}`, // Token for authentication
          },
        });
        if (!response.ok) throw new Error("Failed to fetch order details.");
        this.selectedOrder = await response.json();
      } catch (error) {
        console.error(error);
        alert(error.message);
      }
    },

    // Update order details
    async updateOrder() {
      if (!this.selectedOrder) return;

      try {
        const response = await fetch(`https://localhost:7131/api/orders/${this.selectedOrder.orderId}`, {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${this.$store.state.token}`, // Token for authentication
          },
          body: JSON.stringify(this.selectedOrder),
        });
        if (!response.ok) throw new Error("Failed to update order.");
        alert("Order updated successfully.");
        this.fetchOrders(); // Refresh orders list
        this.selectedOrder = null; // Clear selected order
      } catch (error) {
        console.error(error);
        alert(error.message);
      }
    },

    // Delete an order
    async deleteOrder(orderId) {
      if (!confirm("Are you sure you want to delete this order?")) return;

      try {
        const response = await fetch(`https://localhost:7131/api/orders/${orderId}`, {
          method: "DELETE",
          headers: {
            "Authorization": `Bearer ${this.$store.state.token}`, // Token for authentication
          },
        });
        if (!response.ok) throw new Error("Failed to delete order.");
        alert("Order deleted successfully.");
        this.fetchOrders(); // Refresh orders list
      } catch (error) {
        console.error(error);
        alert(error.message);
      }
    },

    // Update order status
    async updateOrderStatus(order) {
      try {
        console.log("Updating order status:", order); // Debug: log order object
        const response = await fetch(`https://localhost:7131/api/orders/${order.orderId}/status`, {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${this.$store.state.token}`, // Token for authentication
          },
          body: JSON.stringify(order.status), // Send only status as a string
        });

        if (!response.ok) {
          const errorResponse = await response.json();
          console.error("Error response:", errorResponse); // Debug: log error details
          throw new Error(errorResponse.message || "Failed to update order status.");
        }

        alert("Order status updated successfully.");
      } catch (error) {
        console.error(error);
        alert(error.message);
      }
    },

  },
 


};
</script>

<style scoped>
.admin-orders {
  padding: 20px;
}

table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 20px;
}

th,
td {
  border: 1px solid #ddd;
  padding: 8px;
  text-align: center;
}

th {
  background-color: #f4f4f4;
}

.selected {
  background-color: #e0f7fa;
}
button {
    background-color: var(--mainblue);
    color: white;
    border: none;
 margin: 1px;
    cursor: pointer;
    border-radius: 5px;
   height: 2rem;
    transition: background-color 0.3s ease;
 
 
  }
  
  button:hover {
    background-color: #0056b3;
  }
.disable-btn{
  background-color: #f8f9fa;
  color: #6c757d;
}
.disable-btn:hover{
  background-color: #f8f9fa;
  color: #6c757d;
  cursor:auto;
}
  
</style>
