# Definition for singly-linked list.
class ListNode:
    def __init__(self, val=0, next=None):
        self.val = val
        self.next = next
class Solution:
    def removeElements(self, head: ListNode, val: int) -> ListNode:
        if head == None:
            return None

        while head.val == val:
            if head.next is None:
                return None
            head = head.next
            
        ptr = head
        while ptr.next is not None:
            if ptr.next.val == val:
                ptr.next = ptr.next.next
            else:
                ptr = ptr.next

        return head
            
sol = Solution()
# [1,2,6,3,4,5,6]
# 6
# [6,6,6]
# 6
# [6,5,6]
# 6
# [1]
# 2
input = ListNode(1,ListNode(2,ListNode(6,ListNode(3,ListNode(4,ListNode(5,ListNode(6)))))))
result = sol.removeElements(input, 6)
input = ListNode(6,ListNode(6,ListNode(6)))
result = sol.removeElements(input, 6)
input = ListNode(6,ListNode(5,ListNode(6)))
result = sol.removeElements(input, 6)
input = ListNode(1)

result = sol.removeElements(input, 2)
