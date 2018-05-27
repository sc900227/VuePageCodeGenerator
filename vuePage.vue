<style lang="less">
</style>

<template>
    <div>
      <Card>
        <p slot="title">
          <Icon type="help-buoy"></Icon>基因管理
        </p>
            <crud-table :table-data="tableData" :columns="columns" :total="total" :page-option="pageOption"
                          :on-ok="handleOk" 
                          :table-loading="tableLoading"
                          @on-pre-add="handlePreAdd"
                          @on-pre-edit="handlePreEdit"
                          @on-page-change="handlePageChange"
                          @on-sort-change="handleSort"
                          @on-delete="handleDelete">
                  <Form slot="modal-content" ref="crudItem"
                        :model="crudItem" 
                        :rules="ruleValidate" label-position="right" :label-width="100">
                        <FormItem label='"基因名称"' prop='GeneName'>
                            <Input v-model='crudItem.GeneName' placeholder = ''></Input>
                        </FormItem>
                        <FormItem label='"检测方法"' prop='TestMethod'>
                            <Input v-model='crudItem.TestMethod' placeholder = ''></Input>
                        </FormItem> 
                        <FormItem label='"基因型野生"' prop='GeneTypeW'>
                            <Input v-model='crudItem.GeneTypeW' placeholder = ''></Input>
                            </FormItem> 
                        <FormItem label='"基因型杂合"' prop='GeneTypeH'>
                            <Input v-model='crudItem.GeneTypeH' placeholder = ''></Input>
                        </FormItem> 
                        <FormItem label='"基因型突变"' prop='GeneTypeM'>
                            <Input v-model='crudItem.GeneTypeM' placeholder = ''></Input>
                        </FormItem> 
                        <FormItem label='"基因型未知"' prop='GeneTypeX'>
                            <Input v-model='crudItem.GeneTypeX' placeholder = ''></Input>
                        </FormItem> 
                  </Form>
                  <Row slot="filter" :gutter="16">
                    <Col span="8">
                        <Input v-model="filterEnter.textInput" placeholder="试剂盒名称..."></Input>
                    </Col>
                    
                    <Button type="primary" icon="search" @click="fetchData">搜索</Button>
                    
                </Row>
              </crud-table>
      </Card>
    </div>
</template>

<script>
import CrudTable from "../common/crudTable.vue";

export default {
  name: "reagentinfo-page",
  components: {
    CrudTable
  },
  data() {
    return {
      columns: [
        [
          {
            title: '"基因名称"',
            key: "GeneName",
            sortable: "custom",
            handle: null
          },
          {
            title: '"检测方法"',
            key: "TestMethod",
            sortable: "custom",
            handle: null
          },
          {
            title: '"基因型野生"',
            key: "GeneTypeW",
            sortable: "custom",
            handle: null
          },
          {
            title: '"基因型杂合"',
            key: "GeneTypeH",
            sortable: "custom",
            handle: null
          },
          {
            title: '"基因型突变"',
            key: "GeneTypeM",
            sortable: "custom",
            handle: null
          },
          {
            title: '"基因型未知"',
            key: "GeneTypeX",
            sortable: "custom",
            handle: null
          },
          {
            title: '"试剂ID"',
            key: "ReagentID",
            sortable: "custom",
            handle: null
          },
          {
            title: "操作",
            key: "action",
            sortable: null,
            handle: "['edit', 'delete']"
          }
        ]
      ],
      tableData: [],
      pageOption: { pageIndex: 1, pageSize: 10 },
      total: 0,
      tableLoading: true,
      filterEnter: {},
      orderBy: "ID",
      url: "api/services/app/ReagentInfo/",
      header: {
        headers: { "Content-Type": "application/json" }
      },
      crudItem: {
        Id: null,
        ReagentName: null,
        ForDrug: null
      },
      ruleValidate: {
        GeneName: [
          { required: true, message: '"基因名称"不能为空！', trigger: "blur" }
        ],
        TestMethod: [
          { required: true, message: '"检测方法"不能为空！', trigger: "blur" }
        ],
        GeneTypeW: [
          { required: true, message: '"基因型野生"不能为空！', trigger: "blur" }
        ],
        GeneTypeH: [
          { required: true, message: '"基因型杂合"不能为空！', trigger: "blur" }
        ],
        GeneTypeM: [
          { required: true, message: '"基因型突变"不能为空！', trigger: "blur" }
        ],
        GeneTypeX: [
          { required: true, message: '"基因型未知"不能为空！', trigger: "blur" }
        ],
        ReagentID: [
          { required: true, message: '"试剂ID"不能为空！', trigger: "blur" }
        ]
      }
    };
  },
  mounted() {
    this.fetchData();
  },
  computed: {
    filterKey() {
      let filter = this.filterEnter;
      let strFilter = [];
      let str = {};
      if (filter) {
        if (filter.textInput) {
          str.ColumName = "ReagentName";
          str.ColumValue = filter.textInput;
          strFilter.push(str);
          return strFilter;
        }
        return "";
      } else {
        return "";
      }
    }
  },
  methods: {
    fetchData() {
      let vm = this;

      vm.tableLoading = true;
      let params = {
        Sorting: vm.orderBy || undefined,
        SkipCount: (vm.pageOption.pageIndex - 1) * vm.pageOption.pageSize,
        MaxResultCount: vm.pageOption.pageSize,
        Filters: vm.filterKey || undefined
      };

      vm.$http
        .post(vm.url + "GetReagentInfosPage", params, vm.header)
        .then(response => {
          console.log(response);
          vm.tableData = response.data.Result["Items"];
          vm.total = Number(response.data.Result["TotalCount"]);
          vm.tableLoading = false;
        })
        .catch(error => {
          console.log(error);
        });
    },
    handleSort(order) {
      this.orderBy = order.replace(" ", "-");
      this.fetchData();
    },
    handlePageChange(pageOpt) {
      let vm = this;
      vm.pageOption = JSON.parse(JSON.stringify(pageOpt));
      vm.fetchData();
    },
    handleOk() {
      let vm = this;
      let params = {
        ID: vm.crudItem.Id,
        ReagentName: vm.crudItem.ReagentName,
        ForDrug: vm.crudItem.ForDrug
      };

      return new Promise(resolve => {
        vm.$refs["crudItem"].validate(valid => {
          if (valid) {
            let promise = null;
            vm.crudItem.Id = vm.crudItem.Id || undefined;
            if (!vm.crudItem.Id) {
              promise = vm.$http.post(
                vm.url + "CreateReagentInfo",
                params,
                vm.header
              );
            } else {
              promise = vm.$http.post(
                vm.url + "UpdateReagentInfo",
                params,
                vm.header
              );
            }

            promise
              .then(res => {
                resolve(true);
                vm.fetchData();
              })
              .catch(err => {
                console.log(err);
              });
          } else {
            resolve(false);
          }
        });
      });
    },
    handlePreAdd() {
      let item = this.crudItem;
      //清空所有值
      for (var prop in item) {
        item[prop] = null;
      }
    },
    handlePreEdit(val) {
      let item = this.crudItem;
      //赋值所有值
      for (var prop in item) {
        item[prop] = val[prop];
      }
    },
    handleDelete(val) {
      let vm = this;
      vm.$http
        .delete(vm.url + "DeleteReagentInfo?Id=" + val.Id)
        .then(() => {
          vm.$Message.success("已成功删除");
          vm.fetchData();
        })
        .catch(error => {
          console.log(error);
        });
    }
  }
};
</script>